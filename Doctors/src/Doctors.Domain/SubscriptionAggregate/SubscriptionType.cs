using Ardalis.SmartEnum;

namespace Doctors.Domain.SubscriptionAggregate;

public sealed class SubscriptionType : SmartEnum<SubscriptionType>
{
    public static readonly SubscriptionType Free = new(nameof(Free), 0, 0);
    public static readonly SubscriptionType Basic = new(nameof(Basic), 1, 1);
    public static readonly SubscriptionType Premium = new(nameof(Premium), 2, 5);
    
    public int MaxOffices { get; }

    private SubscriptionType(string name, int value, int maxOffices) : base(name, value)
    {
        MaxOffices = maxOffices;
    }
}
