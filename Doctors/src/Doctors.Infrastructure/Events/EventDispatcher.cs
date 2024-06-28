using Doctors.Application.Common.Events;
using Doctors.Domain.Common;
using FindADoctor.SharedKernel.IntegrationEvents;
using MediatR;

namespace Doctors.Infrastructure.Events;

public class EventDispatcher : IEventDispatcher
{
    private readonly IPublisher _mediator;

    public EventDispatcher(IPublisher mediator)
    {
        _mediator = mediator;
    }

    public Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        var notification = GetNotificationEvent(domainEvent);
        return _mediator.Publish(notification);
    }
    
    public async Task DispatchIntegrationAsync(IntegrationEvent integrationEvent)
    {
        await _mediator.Publish(integrationEvent);
    }
    
    private static INotification GetNotificationEvent(IDomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType();

        var notification =
            Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(eventType), domainEvent) as
                INotification;

        return notification!;
    }
}