namespace Appointments.Domain.AppointmentAggregate;

public record AppointmentId
{
    public Guid Value { get; }
    
    private AppointmentId(Guid value)
    {
        Value = value;
    }
    
    public static AppointmentId New() => new(Guid.NewGuid());
    
    public static AppointmentId From(Guid value) => new(value);
}