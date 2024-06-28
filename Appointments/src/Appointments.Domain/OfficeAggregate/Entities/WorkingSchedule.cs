using Appointments.Domain.Common;
using Appointments.Domain.Common.ValueObjects;

namespace Appointments.Domain.OfficeAggregate.Entities;

public class WorkingSchedule : Entity<WorkingScheduleId>
{
    public Dictionary<DateOnly, List<TimeRange>> Calendar { get; } = new();

    public WorkingSchedule (Dictionary<DateOnly, TimeRange> workingCalendar, WorkingScheduleId? id = null) : base(id ?? WorkingScheduleId.New())
    {
        var calendar = workingCalendar
            .ToDictionary(kvp => kvp.Key, kvp => CalculateTimeSlots(kvp.Value.Start, kvp.Value.End));

        Calendar = calendar;
    }
    
    public static WorkingSchedule Empty() => new (id: WorkingScheduleId.New());
    
    public bool TimeSlotAvailable(DateOnly date, TimeRange time)
    {
        if (!Calendar.TryGetValue(date, out var timeSlots))
            return false;
        
        return timeSlots.Any(timeSlot => timeSlot == time);
    }
    
    private static List<TimeRange> CalculateTimeSlots(TimeOnly startTime, TimeOnly endTime)
    {
        var timeSlots = new List<TimeRange>();
        var currentTime = startTime;
        
        while (currentTime < endTime)
        {
            var finishTime = currentTime.AddMinutes(30);
            
            if(endTime >= finishTime)
                timeSlots.Add(new TimeRange(currentTime, finishTime)); // Each appointment is 30 minutes
            
            currentTime = finishTime;
        }
        
        return timeSlots;
    }
    
    private WorkingSchedule(
        Dictionary<DateOnly, List<TimeRange>>? calendar = null, WorkingScheduleId? id = null) : base(id ?? WorkingScheduleId.New())
    {
        Calendar = calendar ?? new Dictionary<DateOnly, List<TimeRange>>();
    }
    
    private WorkingSchedule() { }
}