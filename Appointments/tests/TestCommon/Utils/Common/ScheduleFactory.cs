using Appointments.Domain.Common.Entities;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate.Entities;
using TestCommon.TestConstants;

namespace TestCommon.Utils.Common;

public static class ScheduleFactory
{
    public static WorkingSchedule Create(Dictionary<DateOnly, TimeRange>? workingCalendar = null, WorkingScheduleId? id = null)
        => new(
            workingCalendar ?? new Dictionary<DateOnly, TimeRange> { { Constants.WorkingSchedule.Date, Constants.WorkingSchedule.TimeRange } },
            id ?? Constants.WorkingSchedule.Id
        );
}