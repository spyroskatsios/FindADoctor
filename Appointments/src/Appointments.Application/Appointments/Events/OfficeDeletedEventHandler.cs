using Appointments.Application.Common.Events;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.OfficeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Application.Appointments.Events;

public class OfficeDeletedEventHandler : INotificationHandler<DomainEventNotification<OfficeDeletedEvent>>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IAppointmentWriteRepository _appointmentWriteRepository;

    public OfficeDeletedEventHandler(IAppointmentWriteRepository appointmentWriteRepository, IReadDbContext readDbContext)
    {
        _appointmentWriteRepository = appointmentWriteRepository;
        _readDbContext = readDbContext;
    }

    public async Task Handle(DomainEventNotification<OfficeDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var appointments = await _readDbContext.Appointments
            .Where(x => x.OfficeId == notification.DomainEvent.OfficeId)
            .ToListAsync(cancellationToken);
        
        foreach (var appointment in appointments)
        {
            appointment.Cancel();
        }
        
        await _appointmentWriteRepository.UpdateRangeAsync(appointments, cancellationToken);
    }
}