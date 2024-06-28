namespace Appointments.Domain.Common.ValueObjects;


public record BookedScheduleId
{
    public Guid Value { get; }
    
    private BookedScheduleId(Guid value)
    {
        Value = value;
    }
    
    public static BookedScheduleId New() => new(Guid.NewGuid());
    
    public static BookedScheduleId From(Guid value) => new(value);
}