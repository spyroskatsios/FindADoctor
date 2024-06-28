using Appointments.Domain.AppointmentAggregate;

namespace TestCommon.TestConstants;

public partial class Constants
{
    public static class Appointment
    {
        public static readonly AppointmentId Id = AppointmentId.New();
        public static readonly DateOnly Date = DateOnly.Parse("2022-01-01");
        public static readonly TimeOnly Time = TimeOnly.Parse("10:00");
        public static readonly DateTime DateTime = Date.ToDateTime(Time);
    }
}