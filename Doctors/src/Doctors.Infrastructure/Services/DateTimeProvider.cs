using Doctors.Domain.Common.Interfaces;

namespace Doctors.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public TimeOnly TimeOnly => TimeOnly.FromDateTime(UtcNow);
    public DateOnly DateOnly => DateOnly.FromDateTime(UtcNow);
}