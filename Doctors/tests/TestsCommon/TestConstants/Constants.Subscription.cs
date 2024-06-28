using Doctors.Domain.SubscriptionAggregate;
using SubscriptionType = Doctors.Contracts.Subscriptions.SubscriptionType;

namespace TestsCommon.TestConstants;

public static partial class Constants
{
    public static class Subscription
    {
        public static readonly SubscriptionId Id = SubscriptionId.New();
        public static readonly SubscriptionType SubscriptionType = SubscriptionType.Premium;
    }
}