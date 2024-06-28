namespace Appointments.Domain.PatientAggregate;

public record PatientId
{
    public Guid Value { get; }
    
    private PatientId(Guid value)
    {
        Value = value;
    }
    
    public static PatientId New() => new(Guid.NewGuid());
    
    public static PatientId From(Guid value) => new(value);
}