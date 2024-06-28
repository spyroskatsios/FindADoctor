using System.Diagnostics;
using Doctors.Contracts.Subscriptions;
using Doctors.Domain.SubscriptionAggregate;
using SubscriptionType = Doctors.Contracts.Subscriptions.SubscriptionType;

namespace Doctors.Api.Mapping;

public static class SubscriptionMappers
{
    public static SubscriptionResponse ToSubscriptionResponse(this Subscription subscription)
        => new(subscription.Id.Value, subscription.SubscriptionType.ToResponse());

    private static SubscriptionType ToResponse(
        this Doctors.Domain.SubscriptionAggregate.SubscriptionType subscriptionType)
        => subscriptionType.Name switch
        {
            nameof(Domain.SubscriptionAggregate.SubscriptionType.Free) => SubscriptionType.Free,
            nameof(Domain.SubscriptionAggregate.SubscriptionType.Basic) => SubscriptionType.Basic,
            nameof(Domain.SubscriptionAggregate.SubscriptionType.Premium) => SubscriptionType.Premium,
            _ => throw new UnreachableException()
        };
}
    
   