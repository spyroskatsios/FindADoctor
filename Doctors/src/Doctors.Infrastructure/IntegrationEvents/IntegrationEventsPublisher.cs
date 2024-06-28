using System.Text;
using System.Text.Json;
using Doctors.Infrastructure.Settings;
using FindADoctor.SharedKernel.IntegrationEvents;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Doctors.Infrastructure.IntegrationEvents;

public interface IIntegrationEventsPublisher
{
    void PublishEvent(IntegrationEvent integrationEvent);
}

public class IntegrationEventsPublisher : IIntegrationEventsPublisher
{
    private readonly RabbitMQSettings _rabbitMQSettings;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public IntegrationEventsPublisher(IOptions<RabbitMQSettings> rabbitMQSettings)
    {
        _rabbitMQSettings = rabbitMQSettings.Value;
        var connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMQSettings.HostName,
            Port = _rabbitMQSettings.Port,
            UserName = _rabbitMQSettings.UserName,
            Password = _rabbitMQSettings.Password
        };

        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_rabbitMQSettings.ExchangeName, ExchangeType.Fanout, durable: true);
    }
    
    public void PublishEvent(IntegrationEvent integrationEvent)
    {
        var serializedIntegrationEvent = JsonSerializer.Serialize(integrationEvent);
        
        var body = Encoding.UTF8.GetBytes(serializedIntegrationEvent);

        _channel.BasicPublish(
            exchange: _rabbitMQSettings.ExchangeName,
            routingKey: string.Empty,
            body: body);
    }
}