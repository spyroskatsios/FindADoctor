namespace Doctors.Domain.Common.Interfaces;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
    public TimeOnly TimeOnly { get; }
    public DateOnly DateOnly { get; }
}