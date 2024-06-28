namespace Appointments.Contracts.Schedule;

public record ScheduleResponse(List<CalendarDto> Calendar);

public record CalendarDto(DateTime Date, List<TimeRangeDto> TimeRange);