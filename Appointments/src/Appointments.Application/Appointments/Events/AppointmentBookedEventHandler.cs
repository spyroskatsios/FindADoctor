using Appointments.Application.Common.Events;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.OfficeAggregate;
using MediatR;

namespace Appointments.Application.Appointments.Events;

public class AppointmentBookedEventHandler : INotificationHandler<DomainEventNotification<AppointmentBookedEvent>>
{
    private readonly IAppointmentWriteRepository _appointmentWriteRepository;

    public AppointmentBookedEventHandler(IAppointmentWriteRepository appointmentWriteRepository)
    {
        _appointmentWriteRepository = appointmentWriteRepository;
    }


    public async Task Handle(DomainEventNotification<AppointmentBookedEvent> notification, CancellationToken cancellationToken)
    {
       await _appointmentWriteRepository.CreateAsync(notification.DomainEvent.Appointment, cancellationToken);
    }
}