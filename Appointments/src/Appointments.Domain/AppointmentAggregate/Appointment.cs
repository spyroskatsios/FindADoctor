using Appointments.Domain.Common;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate;
using Appointments.Domain.PatientAggregate;
using ErrorOr;

namespace Appointments.Domain.AppointmentAggregate;

public class Appointment : AggregateRoot<AppointmentId>
{
    public OfficeId OfficeId { get; }
    public PatientId PatientId { get; }
    public DoctorId DoctorId { get; }

    public DateOnly Date;
    public TimeRange TimeRange { get; }
    public AppointmentStatus Status { get; private set; }
    

    public Appointment(OfficeId officeId, PatientId patientId, DoctorId doctorId, DateOnly date, TimeOnly time, AppointmentId? id = null)
        : base(id ?? AppointmentId.New())
    {
        OfficeId = officeId;
        PatientId = patientId;
        DoctorId = doctorId;
        Date = date;
        TimeRange = new TimeRange(time, time.AddMinutes(30));
        Status = AppointmentStatus.Pending;
    }

    public ErrorOr<Success> Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            return AppointmentErrors.CannotConfirm;
    
        Status = AppointmentStatus.Confirmed;
        _domainEvents.Add(new AppointmentApprovedEvent(this));
        return Result.Success;
    }

    public ErrorOr<Success> Reject()
    {
        if (Status != AppointmentStatus.Pending)
            return AppointmentErrors.CannotReject;
    
        Status = AppointmentStatus.Rejected;
        _domainEvents.Add(new AppointmentRejectedEvent(this));
        return Result.Success;
    }

    public ErrorOr<Success> Cancel()
    {
        if (Status != AppointmentStatus.Confirmed && Status != AppointmentStatus.Pending)
            return AppointmentErrors.CannotCancel;
    
        Status = AppointmentStatus.Cancelled;
        _domainEvents.Add(new AppointmentCancelledEvent(this));
        return Result.Success;
    }
    
    public ErrorOr<Success> Complete()
    {
        if (Status != AppointmentStatus.Confirmed)
            return AppointmentErrors.CannotCancel;
    
        Status = AppointmentStatus.Completed;
        _domainEvents.Add(new AppointmentCompletedEvent(this));
        return Result.Success;
    }
    
#pragma warning disable CS8618
    private Appointment(){}
#pragma warning restore CS8618
    
}