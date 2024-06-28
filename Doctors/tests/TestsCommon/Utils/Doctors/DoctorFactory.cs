using Doctors.Domain.DoctorAggregate;

namespace TestsCommon.Utils.Doctors;

public static class DoctorFactory
{
    public static Doctor Create(string? firstName = null, string? lastName = null,
        Speciality? speciality = null, string? userId = null, DoctorId? id = null)
        => new(
            firstName ?? TestConstants.Constants.Doctor.FirstName,
            lastName ?? TestConstants.Constants.Doctor.LastName,
            speciality ?? TestConstants.Constants.Doctor.Speciality,
            userId ?? TestConstants.Constants.User.Id,
            id ?? TestConstants.Constants.Doctor.Id);
}