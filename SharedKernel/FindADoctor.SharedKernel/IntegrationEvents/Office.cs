namespace FindADoctor.SharedKernel.IntegrationEvents;

public record OfficeCreatedIntegrationEvent(Guid OfficeId, Guid DoctorId) : IntegrationEvent;

public record OfficeRemovedIntegrationEvent(Guid OfficeId) : IntegrationEvent;