using Appointments.Domain.PatientAggregate;

namespace TestCommon.TestConstants;

public partial class Constants
{
    public static class Patient
    {
        public static readonly PatientId Id = PatientId.New();
        
    }
}