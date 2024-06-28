using Doctors.Domain.Common;
using Doctors.Domain.DoctorAggregate;

namespace Doctors.Domain.SubscriptionAggregate;

public class Subscription : AggregateRoot<SubscriptionId>
{
    public SubscriptionType SubscriptionType { get; }
    public DoctorId DoctorId { get; }
    

    public Subscription(SubscriptionType subscriptionType, DoctorId doctorId, SubscriptionId? id = null)
        : base(id ?? SubscriptionId.New())
    {
        SubscriptionType = subscriptionType;
        DoctorId = doctorId;
    }
    
#pragma warning disable CS8618
    private Subscription () { }
#pragma warning restore CS8618 
    
}