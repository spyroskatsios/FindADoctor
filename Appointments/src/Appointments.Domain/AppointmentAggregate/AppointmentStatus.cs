using Ardalis.SmartEnum;

namespace Appointments.Domain.AppointmentAggregate;

public sealed class AppointmentStatus : SmartEnum<AppointmentStatus>
{
    public static readonly AppointmentStatus Pending = new(nameof(Pending), 0);
    public static readonly AppointmentStatus Confirmed = new(nameof(Confirmed), 1);
    public static readonly AppointmentStatus Rejected = new(nameof(Rejected), 2);
    public static readonly AppointmentStatus Cancelled = new(nameof(Cancelled), 3);
    public static readonly AppointmentStatus Completed = new(nameof(Cancelled), 3);
    
    private AppointmentStatus(string name, int value) : base(name, value)
    {
    }
}