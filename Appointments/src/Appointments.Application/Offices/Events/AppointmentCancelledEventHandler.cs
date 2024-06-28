using Appointments.Application.Common.Events;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.AppointmentAggregate;
using MediatR;
using Throw;

namespace Appointments.Application.Offices.Events;

public class AppointmentCancelledEventHandler : INotificationHandler<DomainEventNotification<AppointmentCancelledEvent>>
{
    private readonly IOfficeWriteRepository _officeWriteRepository;

    public AppointmentCancelledEventHandler(IOfficeWriteRepository officeWriteRepository)
    {
        _officeWriteRepository = officeWriteRepository;
    }

    public async Task Handle(DomainEventNotification<AppointmentCancelledEvent> notification, CancellationToken cancellationToken)
    {
        var office = await _officeWriteRepository.GetAsync(notification.DomainEvent.Appointment.OfficeId, cancellationToken);

        office.ThrowIfNull();
        
        office.RemoveAppointment(notification.DomainEvent.Appointment);
        
        await _officeWriteRepository.UpdateAsync(office, cancellationToken);
    }
}