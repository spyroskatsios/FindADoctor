using Appointments.Application.Common.Events;
using Appointments.Domain.Common;
using FindADoctor.SharedKernel.IntegrationEvents;
using MediatR;

namespace Appointments.Infrastructure.Events;

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
    
    private static INotification GetNotificationEvent(IDomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType();

        var notification =
            Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(eventType), domainEvent) as
                INotification;

        return notification!;
    }
}