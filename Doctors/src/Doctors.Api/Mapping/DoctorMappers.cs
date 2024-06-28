using System.Diagnostics;
using Doctors.Application.Common.Filtering;
using Doctors.Application.Doctors.Queries;
using Doctors.Contracts.Doctors;
using Doctors.Domain.DoctorAggregate;
using Speciality = Doctors.Contracts.Doctors.Speciality;

namespace Doctors.Api.Mapping;

public static class DoctorMappers
{
    public static DoctorResponse ToDoctorResponse(this Doctor doctor) 
        => new(doctor.Id.Value, doctor.FirstName, doctor.LastName, doctor.Speciality.ToResponse());
    
    public static SearchDoctorsQuery ToSearchDoctorsQuery(this SearchDoctorsRequest request) 
        => new(request.FirstName, request.LastName, 
            request.Speciality is null ? null : Doctors.Domain.DoctorAggregate.Speciality.FromName(request.Speciality.ToString()),
            request.PageSize ?? 10, request.PageNumber ?? 1, (EnSortOrder?)request.SortOrder ?? EnSortOrder.Ascending, request.SortBy);
    
    private static Speciality ToResponse(this Doctors.Domain.DoctorAggregate.Speciality speciality)
        => speciality.Name switch
        {
            nameof(Doctors.Domain.DoctorAggregate.Speciality.Cardiologist) => Speciality.Cardiologist,
            nameof(Doctors.Domain.DoctorAggregate.Speciality.Dermatologist) => Speciality.Dermatologist,
            nameof(Doctors.Domain.DoctorAggregate.Speciality.Endocrinologist) => Speciality.Endocrinologist,
            nameof(Doctors.Domain.DoctorAggregate.Speciality.Gastroenterologist) => Speciality.Gastroenterologist,
            nameof(Doctors.Domain.DoctorAggregate.Speciality.Nephrologist) => Speciality.Nephrologist,
            nameof(Doctors.Domain.DoctorAggregate.Speciality.Neurologist) => Speciality.Neurologist,
            _ => throw new UnreachableException()
        };
}