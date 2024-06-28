using Appointments.Application.Offices.Commands;
using Appointments.Contracts.Schedule;
using Appointments.Domain.Common.ValueObjects;

namespace Appointments.Api.Mapping;

public static class ScheduleMapping
{
    public static AddScheduleCommand ToAddScheduleCommand(this AddScheduleRequest request,  Guid officeId)
        => new(officeId, request.WorkingCalendar.Select(
                x => new KeyValuePair<DateOnly, TimeRange>(DateOnly.FromDateTime(x.Date),
                    new TimeRange(TimeOnly.FromDateTime(x.TimeRange.Start), TimeOnly.FromDateTime(x.TimeRange.End))))
            .ToList().ToDictionary());

    public static ScheduleResponse ToScheduleResponse(this Dictionary<DateOnly, List<TimeRange>> schedule)
    {
        var calendar = new List<CalendarDto>();
        
        foreach (var kvp in schedule)
        {
            var date = kvp.Key.ToDateTime(TimeOnly.MinValue);

            var timeRanges = new List<TimeRangeDto>();
            
            foreach (var timeRange in kvp.Value)
            {
                var start = date.Add(timeRange.Start.ToTimeSpan());
                var end = date.Add(timeRange.End.ToTimeSpan());
                timeRanges.Add(new TimeRangeDto(start, end));
            }
            
            calendar.Add(new CalendarDto(date, timeRanges));
        }
        
        return new ScheduleResponse(calendar);
    }
}