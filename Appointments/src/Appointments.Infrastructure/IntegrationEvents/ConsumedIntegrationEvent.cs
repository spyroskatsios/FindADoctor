namespace Appointments.Infrastructure.IntegrationEvents;

public record ConsumedIntegrationEvent(Guid Id, string Handler);