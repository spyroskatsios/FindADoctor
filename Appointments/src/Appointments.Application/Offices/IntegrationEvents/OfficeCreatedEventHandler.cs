using Appointments.Application.Common.Events;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate;
using FindADoctor.SharedKernel.IntegrationEvents;
using MediatR;

namespace Appointments.Application.Offices.IntegrationEvents;

public class OfficeCreatedEventHandler : INotificationHandler<OfficeCreatedIntegrationEvent>
{
    private readonly IOfficeWriteRepository _officeWriteRepository;

    public OfficeCreatedEventHandler(IOfficeWriteRepository officeWriteRepository)
    {
        _officeWriteRepository = officeWriteRepository;
    }

    public async Task Handle(OfficeCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var office = new Office(DoctorId.From(notification.DoctorId), OfficeId.From(notification.OfficeId));
        await _officeWriteRepository.CreateAsync(office, cancellationToken);
    }
}

