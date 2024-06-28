namespace Appointments.Domain.OfficeAggregate;

public record OfficeId
{
    public Guid Value { get; }
    
    private OfficeId(Guid value)
    {
        Value = value;
    }
    
    public static OfficeId New() => new(Guid.NewGuid());
    
    public static OfficeId From(Guid value) => new(value);
}