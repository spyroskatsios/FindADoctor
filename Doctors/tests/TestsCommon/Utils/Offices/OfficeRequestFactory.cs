using Doctors.Contracts.Offices;

namespace TestsCommon.Utils.Offices;

public static class OfficeRequestFactory
{
    public static CreateOfficeRequest CreateCreateOfficeRequest(string? state = null, string? city = null,
        string? street = null, string? streetNumber = null, string? zipCode = null, Guid? doctorId = null)
        => new(state ?? Constants.Address.State, city ?? Constants.Address.City,
            street ?? Constants.Address.Street, streetNumber ?? Constants.Address.StreetNumber,
            zipCode ?? Constants.Address.ZipCode);
}