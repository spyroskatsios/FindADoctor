using Doctors.Contracts.Common;

namespace Doctors.Contracts.Doctors;

public record CreateDoctorRequest(string FirstName, string LastName, Speciality Speciality);

public record SearchDoctorsRequest(string? FirstName, string? LastName, Speciality? Speciality, int? PageSize, int? PageNumber, EnSortOrder? SortOrder, string? SortBy);