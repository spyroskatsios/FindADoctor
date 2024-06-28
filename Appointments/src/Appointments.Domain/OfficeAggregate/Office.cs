using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.Common;
using Appointments.Domain.Common.Entities;
using Appointments.Domain.Common.Interfaces;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate.Entities;
using ErrorOr;

namespace Appointments.Domain.OfficeAggregate;

public class Office : AggregateRoot<OfficeId>
{
    public DoctorId DoctorId { get; }
    private readonly List<AppointmentId> _appointmentIds = [];
    
    public IReadOnlyList<AppointmentId> AppointmentIds => _appointmentIds.AsReadOnly();
    public WorkingSchedule WorkingSchedule { get; private set; } = WorkingSchedule.Empty();
    public BookedSchedule BookedSchedule { get; } = new();
    
    public bool Deleted { get; private set; }

    public Office(DoctorId doctorId, OfficeId? id = null)
        : base(id ?? OfficeId.New())
    {
        DoctorId = doctorId;
    }

    public void AddSchedule(Dictionary<DateOnly, TimeRange> workingCalendar)
    {
        WorkingSchedule = new WorkingSchedule(workingCalendar);
    }
    

    public ErrorOr<Success> BookAppointment(Appointment appointment, IDateTimeProvider dateTimeProvider)
    {
        if (IsAppointmentInPast(appointment, dateTimeProvider)) 
            return OfficeErrors.CannotBookAppointmentInPast;

        if (!WorkingSchedule.TimeSlotAvailable(appointment.Date, appointment.TimeRange))
            return OfficeErrors.CannotBookNotAvailableAppointment;
        
        if(!BookedSchedule.TimeSlotAvailable(appointment.Date, appointment.TimeRange)) 
            return OfficeErrors.CannotBookNotAvailableAppointment;
        
        _appointmentIds.Add(appointment.Id);
        _domainEvents.Add(new AppointmentBookedEvent(appointment));
        BookedSchedule.AddTimeSlot(appointment.Date, appointment.TimeRange);
        
        return Result.Success;
    }
    
    public void RemoveAppointment(Appointment appointment) // TODO: Add error 
    {
        _appointmentIds.Remove(appointment.Id);
        BookedSchedule.Calendar[appointment.Date].Remove(appointment.TimeRange);
    }
    
    public List<TimeRange> GetAvailableTimeSlots(DateOnly date)
    {
        var workingTimeSlots = WorkingSchedule.Calendar[date];

        var bookedTimeSlots = BookedSchedule.Calendar[date];
        
        return workingTimeSlots.Except(bookedTimeSlots).ToList();
    }

    public Dictionary<DateOnly, List<TimeRange>> GetAvailableTimeSlots()
    {
        var availableTimeSlots = new Dictionary<DateOnly, List<TimeRange>>();
        
        foreach (var (date, timeSlots) in WorkingSchedule.Calendar)
        {
            var bookedTimeSlots = BookedSchedule.Calendar.GetValueOrDefault(date, []);
            var availableSlots = timeSlots.Except(bookedTimeSlots).ToList();
            availableTimeSlots[date] = availableSlots;
        }

        return availableTimeSlots;
    }
    
    private static bool IsAppointmentInPast(Appointment appointment, IDateTimeProvider dateTimeProvider)
    {
        if (appointment.Date < dateTimeProvider.DateOnly)
            return true;

        if (appointment.Date == dateTimeProvider.DateOnly && appointment.TimeRange.Start < dateTimeProvider.TimeOnly)
            return true;
        
        return false;
    }
    
    public void Delete()
    {
        _domainEvents.Add(new OfficeDeletedEvent(Id));
        Deleted = true;
    }

#pragma warning disable CS8618
    private Office() { }
#pragma warning restore CS8618

}