using Doctors.Application.Offices.Commands;
using Doctors.Domain.Common.ValueObjects;
using Doctors.Domain.OfficeAggregate;

namespace TestsCommon.Utils.Offices;

public static class OfficeCommandFactory
{
    public static DeleteOfficeCommand CreateDeleteOfficeCommand(Guid? id = null)
        => new(id ?? Constants.Office.Id.Value);
    
    public static CreateOfficeCommand CreateCreateOfficeCommand(string? state = null,  string? city = null,
        string? street = null, string? streetNumber = null, string? zipCode = null, Guid? doctorId = null)
        => new(state ?? Constants.Address.State, city ?? Constants.Address.City,
            street ?? Constants.Address.Street, streetNumber ?? Constants.Address.StreetNumber,
            zipCode ?? Constants.Address.ZipCode, doctorId ?? Constants.Doctor.Id.Value);
}