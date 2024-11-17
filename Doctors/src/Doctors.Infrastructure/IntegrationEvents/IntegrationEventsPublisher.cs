using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Doctors.Infrastructure.Settings;
using FindADoctor.SharedKernel.IntegrationEvents;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;

namespace Doctors.Infrastructure.IntegrationEvents;

public interface IIntegrationEventsPublisher
{
    void PublishEvent(IntegrationEvent integrationEvent, Dictionary<string, string>? extractedContext = null);
}

public class IntegrationEventsPublisher : IIntegrationEventsPublisher
{
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly ConnectionFactory _factory;

    public IntegrationEventsPublisher(IOptions<RabbitMqSettings> rabbitMqSettings)
    {
        _rabbitMqSettings = rabbitMqSettings.Value;
        _factory = new ConnectionFactory
        {
            HostName = _rabbitMqSettings.HostName,
            Port = _rabbitMqSettings.Port,
            UserName = _rabbitMqSettings.UserName,
            Password = _rabbitMqSettings.Password
        };
    }
    
    public void PublishEvent(IntegrationEvent integrationEvent, Dictionary<string, string>? extractedContext = null)
    {
        
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(_rabbitMqSettings.ExchangeName, ExchangeType.Fanout, durable: true);
        
        var serializedIntegrationEvent = JsonSerializer.Serialize(integrationEvent);
        
        var body = Encoding.UTF8.GetBytes(serializedIntegrationEvent);
        
        const string operation = "publish";
        var eventType = integrationEvent!.GetType().Name;
        
        var activityName = $"{eventType} {operation}";
        
        var parentContext = ExtractParentContext(extractedContext!); // Check null
        Baggage.Current = parentContext.Baggage;
        
        using var activity = RabbitMqDiagnostics.ActivitySource.StartActivity(activityName, ActivityKind.Producer, parentContext.ActivityContext);
        
        ActivityContext contextToInject = default;

        if (activity is not null)
        {
            contextToInject = activity.Context;
        }
        else if (Activity.Current is not null)
        {
            contextToInject = Activity.Current.Context;
        }
        
        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 2;
        
        RabbitMqDiagnostics.Propagator.Inject(
            new PropagationContext(contextToInject, Baggage.Current),
            properties,
            InjectTraceContextIntoBasicProperties);

        SetActivityContext(activity, eventType, operation);

        channel.BasicPublish(
            exchange: _rabbitMqSettings.ExchangeName,
            routingKey: string.Empty,
            basicProperties: properties,
            body: body);
    }
    
    private void SetActivityContext(Activity? activity, string eventType, string operation, Activity? parentActivity = null)
    {
        if (activity is null) 
            return;

        activity.SetTag(RabbitMqTags.MessagingSystem, "rabbitmq");
        activity.SetTag(RabbitMqTags.MessagingDestinationKind, _rabbitMqSettings.ExchangeName);
        activity.SetTag(RabbitMqTags.MessagingOperation, operation);
        activity.SetTag(RabbitMqTags.MessagingDestinationName, eventType);
        
    }

    private static void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
    {
        props.Headers ??= new Dictionary<string, object>();
        props.Headers[key] = value;
    }
    
    private static PropagationContext ExtractParentContext(
        Dictionary<string, string> extractedContext)
    {
        var parentContext = RabbitMqDiagnostics.Propagator.Extract(
            default,
            extractedContext,
            Extract);

        return parentContext;
    }
    
    private static IEnumerable<string> Extract(
        Dictionary<string, string> context,
        string key)
    {
        foreach (var entry in context)
        {
            if (entry.Key == key)
                yield return entry.Value;
        }
    }
}
