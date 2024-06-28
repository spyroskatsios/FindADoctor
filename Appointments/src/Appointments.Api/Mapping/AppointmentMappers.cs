using System.Diagnostics;
using Appointments.Application.Appointments.Commands;
using Appointments.Contracts.Appointments;
using Appointments.Domain.AppointmentAggregate;
using AppointmentStatus = Appointments.Contracts.Appointments.AppointmentStatus;

namespace Appointments.Api.Mapping;

public static class AppointmentMappers
{
    public static BookAppointmentCommand ToBookAppointmentCommand(this BookAppointmentRequest request, Guid officeId, Guid patientId)
        => new(patientId, officeId, request.DateTime);
    
    public static AppointmentResponse ToAppointmentResponse(this Appointment appointment)
        => new(appointment.Id.Value, appointment.OfficeId.Value, appointment.PatientId.Value, appointment.DoctorId.Value,
            appointment.Date.ToDateTime(appointment.TimeRange.Start), appointment.Date.ToDateTime(appointment.TimeRange.End), 
            appointment.Status.ToResponse());
    
    private static AppointmentStatus ToResponse(this Appointments.Domain.AppointmentAggregate.AppointmentStatus appointmentStatus)
        => appointmentStatus.Name switch
        {
            nameof(Appointments.Domain.AppointmentAggregate.AppointmentStatus.Pending) => AppointmentStatus.Pending,
            nameof(Appointments.Domain.AppointmentAggregate.AppointmentStatus.Confirmed) => AppointmentStatus.Confirmed,
            nameof(Appointments.Domain.AppointmentAggregate.AppointmentStatus.Rejected) => AppointmentStatus.Rejected,
            nameof(Appointments.Domain.AppointmentAggregate.AppointmentStatus.Cancelled) => AppointmentStatus.Cancelled,
            nameof(Appointments.Domain.AppointmentAggregate.AppointmentStatus.Completed) => AppointmentStatus.Completed,
            _ => throw new UnreachableException()
        };
}