namespace Doctors.Domain.DoctorAggregate;

public record DoctorId
{
    public Guid Value { get; }
    
    private DoctorId(Guid value)
    {
        Value = value;
    }
    
    public static DoctorId New() => new(Guid.NewGuid());
    
    public static DoctorId From(Guid value) => new(value);
}