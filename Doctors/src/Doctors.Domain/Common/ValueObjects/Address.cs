namespace Doctors.Domain.Common.ValueObjects;

public record Address(string State, string City, string Street, string StreetNumber, string ZipCode); // TODO: Add validation