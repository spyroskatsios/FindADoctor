using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.SubscriptionAggregate;

namespace TestsCommon.Utils.Subscriptions;

public static class SubscriptionFactory
{
    public static Subscription Create(SubscriptionType? subscriptionType = null, DoctorId? doctorId = null, SubscriptionId? id = null)
        => new(subscriptionType ?? SubscriptionType.Basic, doctorId ?? TestConstants.Constants.Doctor.Id,
            id ?? TestConstants.Constants.Subscription.Id);
}