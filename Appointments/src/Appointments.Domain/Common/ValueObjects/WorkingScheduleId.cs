namespace Appointments.Domain.Common.ValueObjects;

public record WorkingScheduleId
{
    public Guid Value { get; }
    
    private WorkingScheduleId(Guid value)
    {
        Value = value;
    }
    
    public static WorkingScheduleId New() => new(Guid.NewGuid());
    
    public static WorkingScheduleId From(Guid value) => new(value);
}