using Appointments.Domain.PatientAggregate;
using TestCommon.TestConstants;

namespace TestCommon.Utils.Patients;

public static class PatientFactory
{
    public static Patient Create(string? userId = null, PatientId? id = null)
        => new Patient(userId ?? Constants.User.Id, id ?? Constants.Patient.Id);
}