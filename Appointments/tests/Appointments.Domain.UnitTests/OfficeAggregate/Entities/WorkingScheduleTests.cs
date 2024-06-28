using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate.Entities;

namespace Appointments.Domain.UnitTests.OfficeAggregate.Entities;

public class WorkingScheduleTests
{
    [Fact]
    public void LastTimeSlot_ShouldNotFinish_AfterWorkingHours()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Now);
        var timeStart = TimeOnly.Parse("05:00");
        var timeEnd =  TimeOnly.Parse("06:15");
        
        var time = new TimeRange(timeStart, timeEnd);

        var calendar = new Dictionary<DateOnly, TimeRange> { { date, time }, };
        
        // Act
        var schedule = new WorkingSchedule(calendar);

        // Assert
        var timeSlots = schedule.Calendar.First().Value;

        timeSlots.Count.Should().Be(2);
        timeSlots.Last().End.Should().BeBefore(timeEnd);
    }
}