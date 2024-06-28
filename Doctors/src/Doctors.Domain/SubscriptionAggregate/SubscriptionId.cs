namespace Doctors.Domain.SubscriptionAggregate;


public record SubscriptionId
{
    public Guid Value { get; }
    
    private SubscriptionId(Guid value)
    {
        Value = value;
    }
    
    public static SubscriptionId New() => new(Guid.NewGuid());
    
    public static SubscriptionId From(Guid value) => new(value);
}