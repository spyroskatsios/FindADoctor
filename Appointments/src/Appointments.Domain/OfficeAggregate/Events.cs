using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.Common;

namespace Appointments.Domain.OfficeAggregate;

public record AppointmentBookedEvent(Appointment Appointment) : IDomainEvent;

public record OfficeDeletedEvent(OfficeId OfficeId) : IDomainEvent;