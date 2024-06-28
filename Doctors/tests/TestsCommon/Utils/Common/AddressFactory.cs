using Doctors.Domain.Common.ValueObjects;

namespace TestsCommon.Utils.Common;

public static class AddressFactory
{
    public static Address Create(string? state = null, string? city = null, string? street = null,
        string? streetNumber = null, string? zipCode = null)
        => new(street ?? Constants.Address.Street, streetNumber ?? Constants.Address.StreetNumber,
            city ?? Constants.Address.City, state ?? Constants.Address.State, zipCode ?? Constants.Address.ZipCode);
}