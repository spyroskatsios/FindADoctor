namespace Doctors.Contracts.Doctors;

public record DoctorResponse(Guid Id, string FirstName, string LastName, Speciality Speciality);