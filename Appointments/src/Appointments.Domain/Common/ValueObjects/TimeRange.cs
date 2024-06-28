namespace Appointments.Domain.Common.ValueObjects;

public record TimeRange
{
    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }

    public TimeRange(TimeOnly start, TimeOnly end)
    {
        if(start >= end)
            throw new ArgumentException("End time must be greater than the start time");
        
        Start = start;
        End = end;
    }
    
    public bool OverlapsWith(TimeRange other)
    {
        if (Start >= other.End)
            return false;
        
        if (other.Start >= End)
            return false;

        return true;
    }
}