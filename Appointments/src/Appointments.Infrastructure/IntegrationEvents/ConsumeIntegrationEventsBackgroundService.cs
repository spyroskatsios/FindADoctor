using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Appointments.Application.Common.Interfaces;
using Appointments.Infrastructure.Persistence;
using Appointments.Infrastructure.Settings;
using FindADoctor.SharedKernel.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Throw;

namespace Appointments.Infrastructure.IntegrationEvents;

public class ConsumeIntegrationEventsBackgroundService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ConsumeIntegrationEventsBackgroundService> _logger;
    private readonly CancellationTokenSource _cts;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMQSettings _rabbitMQSettings;

    public ConsumeIntegrationEventsBackgroundService(
        ILogger<ConsumeIntegrationEventsBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<RabbitMQSettings> rabbitMQSettings)
    {
        _logger = logger;
        _cts = new CancellationTokenSource();
        _serviceScopeFactory = serviceScopeFactory;

        _rabbitMQSettings = rabbitMQSettings.Value;

        IConnectionFactory connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMQSettings.HostName,
            Port = _rabbitMQSettings.Port,
            UserName = _rabbitMQSettings.UserName,
            Password = _rabbitMQSettings.Password
        };

        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_rabbitMQSettings.ExchangeName, ExchangeType.Fanout, durable: true);

        _channel.QueueDeclare(
            queue: _rabbitMQSettings.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            _rabbitMQSettings.QueueName,
            _rabbitMQSettings.ExchangeName,
            routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += ConsumeIntegrationEvent;

        _channel.BasicConsume(_rabbitMQSettings.QueueName, autoAck: false, consumer);
    }

    private async void ConsumeIntegrationEvent(object? sender, BasicDeliverEventArgs eventArgs)
    {
        if (_cts.IsCancellationRequested)
        {
            _logger.LogInformation("Cancellation requested, not consuming integration event.");
            return;
        }

        try
        {
            
            var parentContext = RabbitMqDiagnostics.Propagator.Extract(default,
                eventArgs.BasicProperties,
                ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;
            
            const string operation = "process";
            var activityName = $"{eventArgs.RoutingKey} {operation}";

            using var activity = RabbitMqDiagnostics.ActivitySource.StartActivity(activityName, ActivityKind.Consumer,
                parentContext.ActivityContext);
            
            SetActivityContext(activity, eventArgs.RoutingKey, operation);
            
            _logger.LogInformation("Received integration event. Reading message from queue.");

            using var scope = _serviceScopeFactory.CreateScope();

            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var integrationEvent = JsonSerializer.Deserialize<IntegrationEvent>(message);
            integrationEvent.ThrowIfNull();

            _logger.LogInformation(
                "Received integration event of type: {IntegrationEventType}. Publishing event.",
                integrationEvent.GetType().Name);
            
            var dispatcher = scope.ServiceProvider.GetRequiredService<IIntegrationEventDispatcher>();
            await dispatcher.DispatchAsync(integrationEvent);

            _logger.LogInformation("Integration event published successfully. Sending ack to message broker.");

            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred while consuming integration event");
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting integration event consumer background service.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _cts.Dispose();
        return Task.CompletedTask;
    }
    
    private static void SetActivityContext(Activity? activity, string eventName, string operation)
    {
        if (activity is null)
            return;
        
        activity.SetTag(RabbitMqTags.MessagingSystem, "rabbitmq");
        activity.SetTag(RabbitMqTags.MessagingDestinationName, "queue");
        activity.SetTag(RabbitMqTags.MessagingOperation, operation);
        activity.SetTag(RabbitMqTags.MessagingDestinationKind, eventName);
    }
    
    private static IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
    {
        if (!props.Headers.TryGetValue(key, out var value)) 
            return [];

        var bytes = value as byte[];
        
        return [Encoding.UTF8.GetString(bytes!)];
    }
}