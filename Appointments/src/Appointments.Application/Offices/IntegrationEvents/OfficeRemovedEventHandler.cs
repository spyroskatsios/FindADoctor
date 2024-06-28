using Appointments.Application.Common.Events;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.OfficeAggregate;
using Ardalis.GuardClauses;
using FindADoctor.SharedKernel.IntegrationEvents;
using MediatR;

namespace Appointments.Application.Offices.IntegrationEvents;

public class OfficeRemovedEventHandler :  INotificationHandler<OfficeRemovedIntegrationEvent>
{
    private readonly IOfficeWriteRepository _officeWriteRepository;

    public OfficeRemovedEventHandler(IOfficeWriteRepository officeWriteRepository)
    {
        _officeWriteRepository = officeWriteRepository;
    }

    public async Task Handle(OfficeRemovedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var office = await _officeWriteRepository.GetAsync(OfficeId.From(notification.OfficeId), cancellationToken);
        
        Guard.Against.Null(office, nameof(office));
        
        office.Delete();
        
        await _officeWriteRepository.UpdateAsync(office, cancellationToken);
    }
}