using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate;
using TestCommon.TestConstants;
using TestCommon.Utils.Common;

namespace TestCommon.Utils.Offices;

public partial class OfficeFactory
{
    public static Office Create( DoctorId? doctorId = null, OfficeId? id = null)
        => new(doctorId ?? Constants.Doctor.Id, id ?? Constants.Office.Id);

    public static Office CreateWithSchedule( DoctorId? doctorId = null, OfficeId? id = null, Dictionary<DateOnly, TimeRange>? workingCalendar = null)
    {
        var office = new Office(doctorId ?? Constants.Doctor.Id, id ?? Constants.Office.Id);
        office.AddSchedule(workingCalendar ?? new Dictionary<DateOnly, TimeRange> { { Constants.WorkingSchedule.Date, Constants.WorkingSchedule.TimeRange } });
        return office;
    }
}