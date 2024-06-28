namespace Doctors.Contracts.Offices;

public record CreateOfficeRequest(string State, string City, string Street, string StreetNumber, string ZipCode);