using System.Diagnostics;
using System.Text.Json;
using Doctors.Application.Common.Events;
using Doctors.Domain.DoctorAggregate;
using Doctors.Infrastructure.Persistence;
using FindADoctor.SharedKernel.IntegrationEvents;
using MediatR;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

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
        var extractedContext = ExtractTelemetryContextForPersistence();
        
        await _dbContext.OutboxIntegrationEvents.AddAsync(
            new OutboxIntegrationEvent(integrationEvent.GetType().Name,
                JsonSerializer.Serialize(integrationEvent),
                extractedContext));

        await _dbContext.SaveChangesAsync();
    }
    
    private static string? ExtractTelemetryContextForPersistence()
    {
        var activity = Activity.Current;

        if (activity is null)
            return null;

        var extractedContext = new Dictionary<string, string>();

        Propagators.DefaultTextMapPropagator.Inject(
            new PropagationContext(activity.Context, Baggage.Current),
            extractedContext,
            InjectEntry);

        return JsonSerializer.Serialize(extractedContext);
        
    }
    
    private static void InjectEntry(
        Dictionary<string, string> extractedContext,
        string key,
        string value)
        => extractedContext[key] = value;
}