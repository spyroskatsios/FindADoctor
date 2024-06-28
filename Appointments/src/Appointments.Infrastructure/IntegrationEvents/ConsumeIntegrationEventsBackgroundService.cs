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

            _logger.LogInformation("Integration event published in Gym Management service successfully. Sending ack to message broker.");

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
}