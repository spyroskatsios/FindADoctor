using Doctors.Contracts.Subscriptions;

namespace TestsCommon.Utils.Subscriptions;

public static class SubscriptionRequestFactory
{
    public static CreateSubscriptionRequest CreateCreateSubscriptionRequest(SubscriptionType? subscriptionType = null, Guid? doctorId = null)
        => new(subscriptionType ?? Constants.Subscription.SubscriptionType);
}