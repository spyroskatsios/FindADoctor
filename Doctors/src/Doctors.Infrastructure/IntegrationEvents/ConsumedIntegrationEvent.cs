namespace Doctors.Infrastructure.IntegrationEvents;

public record ConsumedIntegrationEvent(Guid Id, string Handler);