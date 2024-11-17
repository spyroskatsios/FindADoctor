namespace Doctors.Infrastructure.IntegrationEvents;

public record OutboxIntegrationEvent(string EventName, string EventContent, string? ActivityExtractedContext);