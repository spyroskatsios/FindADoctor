using ErrorOr;

namespace Appointments.Domain.PatientAggregate;

public static class PatientErrors
{
    public static readonly Error AppointmentAlreadyExists = Error.Validation(
        "Patient.AppointmentAlreadyExists",
        "The appointment already exists");
    
    public static readonly Error AppointmentOverlaps = Error.Validation(
        "Patient.AppointmentOverlaps",
        "There is an appointment at the same date and time range");
}