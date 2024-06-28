namespace Doctors.Contracts.Offices;

public record OfficeResponse(Guid Id, string State, string City, string Street, string StreetNumber, string ZipCode);

public record GetOfficesResponse(List<OfficeResponse> Offices);