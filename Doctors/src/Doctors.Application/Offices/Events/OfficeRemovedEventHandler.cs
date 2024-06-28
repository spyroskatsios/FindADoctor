using Doctors.Application.Common.Events;
using Doctors.Application.Common.Repositories;
using Doctors.Domain.DoctorAggregate;
using MediatR;
using Throw;

namespace Doctors.Application.Offices.Events;

public class OfficeRemovedEventHandler  : INotificationHandler<DomainEventNotification<OfficeRemovedEvent>>
{
    private readonly IOfficeWriteRepository _officeWriteRepository;

    public OfficeRemovedEventHandler(IOfficeWriteRepository officeWriteRepository)
    {
        _officeWriteRepository = officeWriteRepository;
    }

    public async Task Handle(DomainEventNotification<OfficeRemovedEvent> notification, CancellationToken cancellationToken)
    {
        var office = await _officeWriteRepository.GetAsync(notification.DomainEvent.OfficeId, cancellationToken);

        office.ThrowIfNull();
        
        office.Delete();
        
        await _officeWriteRepository.UpdateAsync(office, cancellationToken);
    }
}
