using Appointments.Domain.Common.ValueObjects;

namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class Doctor
    {
        public static readonly DoctorId Id = DoctorId.New();
    }
}