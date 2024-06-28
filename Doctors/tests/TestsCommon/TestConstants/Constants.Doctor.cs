using Doctors.Domain.DoctorAggregate;

namespace TestsCommon.TestConstants;

public static partial class Constants
{
    public static class Doctor
    {
        public static readonly DoctorId Id = DoctorId.New();
        public const string FirstName = "John";
        public const string LastName = "Doe";
        public static readonly Speciality Speciality = Speciality.Cardiologist;
    }
}