namespace Appointments.Infrastructure.IntegrationEvents;

public record OutboxIntegrationEvent(string EventName, string EventContent);