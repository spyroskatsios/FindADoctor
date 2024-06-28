using ErrorOr;

namespace Appointments.Domain.AppointmentAggregate;

public static class AppointmentErrors
{
    public static readonly Error CannotConfirm = Error.Conflict(
        "Appointment.CannotConfirm",
        "You can not confirm an appointment that is not pending");
    
    public static readonly Error CannotReject = Error.Conflict(
        "Appointment.CannotReject",
        "You can not reject an appointment that is not pending");
    
    public static readonly Error CannotCancel = Error.Conflict(
        "Appointment.CannotCancel",
        "You can not cancel an appointment that is not confirmed or pending");
    
    public static readonly Error CannotComplete = Error.Conflict(
        "Appointment.CannotComplete",
        "You can not complete an appointment that is not confirmed");
}