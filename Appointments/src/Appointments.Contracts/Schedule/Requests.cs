namespace Appointments.Contracts.Schedule;

public record AddScheduleRequest(List<DailyCalendarDto> WorkingCalendar);

public record TimeRangeDto(DateTime Start, DateTime End);

public record DailyCalendarDto(DateTime Date, TimeRangeDto TimeRange);