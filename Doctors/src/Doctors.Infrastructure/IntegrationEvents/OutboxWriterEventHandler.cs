using System.Text.Json;
using Doctors.Application.Common.Events;
using Doctors.Domain.DoctorAggregate;
using Doctors.Infrastructure.Persistence;
using FindADoctor.SharedKernel.IntegrationEvents;
using MediatR;

namespace Doctors.Infrastructure.IntegrationEvents;

public class OutboxWriterEventHandler : INotificationHandler<DomainEventNotification<OfficeCreatedEvent>>,
    INotificationHandler<DomainEventNotification<OfficeRemovedEvent>>
{
    private readonly AppDbContext _dbContext;

    public OutboxWriterEventHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DomainEventNotification<OfficeCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new OfficeCreatedIntegrationEvent(notification.DomainEvent.Office.Id.Value, notification.DomainEvent.Doctor.Id.Value);
        await AddOutboxIntegrationEventAsync(integrationEvent);
    }
    
    public async Task Handle(DomainEventNotification<OfficeRemovedEvent> notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new OfficeRemovedIntegrationEvent(notification.DomainEvent.OfficeId.Value);
        await AddOutboxIntegrationEventAsync(integrationEvent);
    }
    
    private async Task AddOutboxIntegrationEventAsync(IntegrationEvent integrationEvent)
    {
        await _dbContext.OutboxIntegrationEvents.AddAsync(
            new OutboxIntegrationEvent(integrationEvent.GetType().Name,
                JsonSerializer.Serialize(integrationEvent)));

        await _dbContext.SaveChangesAsync();
    }
}