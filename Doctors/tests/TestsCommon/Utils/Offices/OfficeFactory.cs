using Doctors.Domain.Common.ValueObjects;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using TestsCommon.Utils.Common;

namespace TestsCommon.Utils.Offices;

public static class OfficeFactory
{
    public static Office Create(Address? address = null, DoctorId? doctorId = null, OfficeId? id = null)
        => new(address ?? AddressFactory.Create(), doctorId ?? Constants.Doctor.Id, id ?? Constants.Office.Id);

}