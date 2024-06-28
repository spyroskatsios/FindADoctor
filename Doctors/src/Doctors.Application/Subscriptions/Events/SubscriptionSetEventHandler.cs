using Doctors.Application.Common.Events;
using Doctors.Application.Common.Repositories;
using Doctors.Domain.DoctorAggregate;
using MediatR;

namespace Doctors.Application.Subscriptions.Events;

public class SubscriptionSetEventHandler : INotificationHandler<DomainEventNotification<SubscriptionSetEvent>>
{
    private readonly ISubscriptionWriteRepository _subscriptionWriteRepository;

    public SubscriptionSetEventHandler(ISubscriptionWriteRepository subscriptionWriteRepository)
    {
        _subscriptionWriteRepository = subscriptionWriteRepository;
    }

    public async Task Handle(DomainEventNotification<SubscriptionSetEvent> notification, CancellationToken cancellationToken)
    {
        await _subscriptionWriteRepository.CreateAsync(notification.DomainEvent.Subscription, cancellationToken);
    }
}