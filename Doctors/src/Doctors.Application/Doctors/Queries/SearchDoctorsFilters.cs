using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.Extensions;

namespace Doctors.Application.Doctors.Queries;

public static class SearchDoctorsFilters
{
    public static IQueryable<Doctor> ApplyDoctorFilters(this IQueryable<Doctor> doctors, SearchDoctorsQuery query)
    {
        if (!query.FirstName.IsNullOrEmpty())
            doctors = doctors.Where(x => x.FirstName.Contains(query.FirstName!));
        
        if (!query.LastName.IsNullOrEmpty())
            doctors = doctors.Where(x => x.LastName.Contains(query.LastName!));

        if(query.Speciality is not null)
            doctors = doctors.Where(x => x.Speciality == query.Speciality);
        
        return doctors;
    }
}