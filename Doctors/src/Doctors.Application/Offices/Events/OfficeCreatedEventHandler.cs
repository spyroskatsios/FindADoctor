using Doctors.Application.Common.Events;
using Doctors.Application.Common.Repositories;
using Doctors.Domain.DoctorAggregate;
using MediatR;

namespace Doctors.Application.Offices.Events;

public class OfficeCreatedEventHandler : INotificationHandler<DomainEventNotification<OfficeCreatedEvent>>
{
    private readonly IOfficeWriteRepository _officeWriteRepository;

    public OfficeCreatedEventHandler(IOfficeWriteRepository officeWriteRepository)
    {
        _officeWriteRepository = officeWriteRepository;
    }

    public async Task Handle(DomainEventNotification<OfficeCreatedEvent> notification, CancellationToken cancellationToken)
    {
        await _officeWriteRepository.CreateAsync(notification.DomainEvent.Office, cancellationToken);
    }
}