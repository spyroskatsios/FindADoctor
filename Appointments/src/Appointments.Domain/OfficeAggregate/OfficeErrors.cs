using ErrorOr;

namespace Appointments.Domain.OfficeAggregate;

public static class OfficeErrors
{
    public static readonly Error CannotBookAppointmentInPast = Error.Validation(
        "Office.CannotBookAppointmentInPast",
        "Cannot book an appointment in the past");
    
    public static readonly Error CannotBookNotAvailableAppointment = Error.Validation(
        "Office.CannotBookNotAvailableAppointment",
        "Cannot book a not available appointment");
}