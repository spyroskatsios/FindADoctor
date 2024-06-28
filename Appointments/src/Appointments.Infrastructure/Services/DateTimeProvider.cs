using Appointments.Domain.Common.Interfaces;

namespace Appointments.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public TimeOnly TimeOnly => TimeOnly.FromDateTime(UtcNow);
    public DateOnly DateOnly => DateOnly.FromDateTime(UtcNow);
}