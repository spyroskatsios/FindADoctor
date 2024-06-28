using Appointments.Domain.Common.ValueObjects;

namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class WorkingSchedule
    {
        public static readonly WorkingScheduleId Id = WorkingScheduleId.New();
        public static readonly DateOnly Date = DateOnly.Parse("2099-01-01");
        public static readonly TimeRange TimeRange = new TimeRange(TimeOnly.Parse("08:00"), TimeOnly.Parse("16:00"));
    }
}
