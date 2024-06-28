using Appointments.Application.Appointments.Commands;
using TestCommon.TestConstants;

namespace TestCommon.Utils.Appointments;

public static class AppointmentCommandFactory
{
    public static BookAppointmentCommand CreateBookAppointmentCommand(Guid? patientId = null, Guid? officeId = null,
        DateTime? dateTime = null)
        => new(patientId ?? Constants.Patient.Id.Value, officeId ?? Constants.Office.Id.Value, dateTime ?? Constants.Appointment.DateTime);
}