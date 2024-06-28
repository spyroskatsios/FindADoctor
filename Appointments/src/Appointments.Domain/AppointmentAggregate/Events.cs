using Appointments.Domain.Common;

namespace Appointments.Domain.AppointmentAggregate;

public record AppointmentApprovedEvent(Appointment Appointment) : IDomainEvent;
public record AppointmentRejectedEvent(Appointment Appointment) : IDomainEvent;
public record AppointmentCancelledEvent(Appointment Appointment) : IDomainEvent;
public record AppointmentCompletedEvent(Appointment Appointment) : IDomainEvent;