using Appointments.Domain.Common.ValueObjects;

namespace Appointments.Domain.Common.Entities;

public class BookedSchedule : Entity<BookedScheduleId>
{
    public Dictionary<DateOnly, List<TimeRange>> Calendar { get; } = new();
    
    public BookedSchedule(BookedScheduleId? id = null) : base(id ?? BookedScheduleId.New()) { }
    
   
    public void AddTimeSlot(DateOnly date, TimeRange timeSlot)
    {
        if (!Calendar.TryGetValue(date, out var timeSlots))
        {
            timeSlots = new List<TimeRange>();
            Calendar[date] = timeSlots;
        }

        timeSlots.Add(timeSlot);
    }
    
    public bool TimeSlotAvailable(DateOnly date, TimeRange time)
    {
        if (!Calendar.TryGetValue(date, out var timeSlots))
            return true;
        
        return timeSlots.All(timeSlot => timeSlot != time);
    }

    public bool Overlaps(DateOnly dateOnly, TimeRange timeRange)
    {
        if(!Calendar.TryGetValue(dateOnly, out var timeSlots))
            return false;
        
        return timeSlots.Any(timeSlot => timeSlot.OverlapsWith(timeRange));
    }
    
    private BookedSchedule() { }
}