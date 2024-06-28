using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate;
using Appointments.Domain.PatientAggregate;
using TestCommon.TestConstants;

namespace TestCommon.Utils.Appointments;

public static class AppointmentFactory
{
    public static Appointment Create(OfficeId? officeId = null, PatientId? patientId = null, DoctorId? doctorId = null,
        DateOnly? date = null, TimeOnly? time = null, AppointmentId? id = null)
        => new(officeId ?? Constants.Office.Id, patientId ?? Constants.Patient.Id, doctorId ?? Constants.Doctor.Id,
            date ?? Constants.Appointment.Date, time ?? Constants.Appointment.Time,
            id ?? Constants.Appointment.Id);
}