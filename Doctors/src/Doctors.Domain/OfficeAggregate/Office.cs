using Doctors.Domain.Common;
using Doctors.Domain.Common.ValueObjects;
using Doctors.Domain.DoctorAggregate;
using ErrorOr;

namespace Doctors.Domain.OfficeAggregate;

public class Office : AggregateRoot<OfficeId>
{
    public Address Address { get; }
    public DoctorId DoctorId { get; }

    public bool Deleted { get; private set; }

    public Office(Address address, DoctorId doctorId, OfficeId? id = null)
        : base(id ?? OfficeId.New())
    {
        Address = address;
        DoctorId = doctorId;
    }
    
    public void Delete() => Deleted = true;


#pragma warning disable CS8618
    private Office() { }
#pragma warning restore CS8618
    
}