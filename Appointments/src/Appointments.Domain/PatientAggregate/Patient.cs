using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.Common;
using Appointments.Domain.Common.Entities;
using Appointments.Domain.Common.ValueObjects;
using ErrorOr;

namespace Appointments.Domain.PatientAggregate;

public class Patient : AggregateRoot<PatientId>
{
    private List<AppointmentId> _appointmentIds = [];
    private readonly BookedSchedule _bookedSchedule = new();
    
    public IReadOnlyList<AppointmentId> AppointmentIds => _appointmentIds.AsReadOnly();
    
    public string UserId { get; }
    
    public Patient(string userId ,PatientId? id = null) : base(id ?? PatientId.New())
    {
        UserId = userId;
    }
    
    public ErrorOr<Success> AddAppointment(Appointment appointment)
    {
        if (HasAppointment(appointment.Id))
            return PatientErrors.AppointmentAlreadyExists;
        
        if(HasOverlappingAppointment(appointment.Date, appointment.TimeRange))
            return PatientErrors.AppointmentOverlaps;
        
        _appointmentIds.Add(appointment.Id);
        _bookedSchedule.AddTimeSlot(appointment.Date, appointment.TimeRange);
        
        return Result.Success;
    }
    
    public bool HasAppointment(AppointmentId appointmentId) => _appointmentIds.Contains(appointmentId);
    
    public bool HasOverlappingAppointment(DateOnly date, TimeRange timeRange) => _bookedSchedule.Overlaps(date, timeRange);
    
    public void RemoveAppointment(Appointment appointment) => _appointmentIds.Remove(appointment.Id); // TODO: Add error
    
   
#pragma warning disable CS8618
    private Patient() { }
#pragma warning restore CS8618
}