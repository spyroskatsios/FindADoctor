using Doctors.Application.Offices.Commands;
using Doctors.Contracts.Offices;
using Doctors.Domain.OfficeAggregate;

namespace Doctors.Api.Mapping;

public static class OfficeMappers
{
    public static CreateOfficeCommand ToCreateOfficeCommand(this CreateOfficeRequest request, Guid doctorId) 
        => new(request.State, request.City, request.Street, request.StreetNumber, request.ZipCode, doctorId);
    
    public static OfficeResponse ToOfficeResponse(this Office office)
        => new(office.Id.Value, office.Address.State, office.Address.City, office.Address.Street, office.Address.StreetNumber, office.Address.ZipCode);
    
    public static GetOfficesResponse ToGetOfficesResponse(this IEnumerable<Office> offices)
        => new(offices.Select(x=>x.ToOfficeResponse()).ToList());
}