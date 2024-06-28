using Doctors.Application.Doctors.Commands;
using Doctors.Domain.DoctorAggregate;

namespace TestsCommon.Utils.Doctors;

public static class DoctorCommandFactory
{
    public static CreateDoctorCommand CreateCreateDoctorCommand(string? firstName = null, string? lastName = null,
        Speciality? speciality = null, string? userId = null)
        => new(
            firstName ?? Constants.Doctor.FirstName,
            lastName ?? Constants.Doctor.LastName,
            speciality ?? Speciality.Cardiologist,
            Constants.Doctor.Id.Value,
            userId ?? Constants.User.Id);
}