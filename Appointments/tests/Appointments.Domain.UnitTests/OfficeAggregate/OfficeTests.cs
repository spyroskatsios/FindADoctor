using Appointments.Domain.Common.Interfaces;
using Appointments.Domain.OfficeAggregate;
using TestCommon.Utils.Appointments;
using TestCommon.Utils.Offices;

namespace Appointments.Domain.UnitTests.OfficeAggregate;

public class OfficeTests
{
     private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    
    [Fact]
    public void BookAppointment_ShouldFail_WhenInPastDate()
    {
        // Arrange
        var office = OfficeFactory.CreateWithSchedule();
        var date = office.WorkingSchedule.Calendar.First().Key;
        var appointment = AppointmentFactory.Create(date: date);
        _dateTimeProvider.DateOnly.Returns(date.AddDays(1));

        // Act
        var result = office.BookAppointment(appointment, _dateTimeProvider);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(OfficeErrors.CannotBookAppointmentInPast);
    }
    
    [Fact]
    public void BookAppointment_ShouldFail_WhenInPastTime()
    {
        // Arrange
        var office = OfficeFactory.CreateWithSchedule();
        var date = office.WorkingSchedule.Calendar.First().Key;
        var time = office.WorkingSchedule.Calendar.First().Value.First().Start;
        var appointment = AppointmentFactory.Create(date: date, time: time); // TODO Add second date
        _dateTimeProvider.DateOnly.Returns(date);
        _dateTimeProvider.TimeOnly.Returns(time.AddHours(1));

        // Act
        var result = office.BookAppointment(appointment, _dateTimeProvider);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(OfficeErrors.CannotBookAppointmentInPast);
    }
    
    
    [Fact]
     public void BookAppointment_ShouldFail_WhenAlreadyBooked()
     {
         // Arrange
         var office = OfficeFactory.CreateWithSchedule();
         var time = office.WorkingSchedule.Calendar.First().Value.First().Start;
         var date = office.WorkingSchedule.Calendar.First().Key;
         var bookedAppointment = AppointmentFactory.Create(date: date, time: time);
         var appointment = AppointmentFactory.Create(date: date, time: time);
         _dateTimeProvider.DateOnly.Returns(date);
         _dateTimeProvider.TimeOnly.Returns(time.AddHours(-1));
         office.BookAppointment(bookedAppointment, _dateTimeProvider);
    
         // Act
         var result = office.BookAppointment(appointment, _dateTimeProvider);
    
         // Assert
         result.IsError.ShouldBeTrue();
         result.FirstError.ShouldBe(OfficeErrors.CannotBookNotAvailableAppointment);
     }
    
     [Fact]
     public void BookAppointment_ShouldFail_WhenNotAvailableDate()
     {
         // Arrange
         var office = OfficeFactory.CreateWithSchedule();
         var time = office.WorkingSchedule.Calendar.Last().Value.First().Start;
         var date = office.WorkingSchedule.Calendar.Last().Key;
         var appointment = AppointmentFactory.Create(date: date.AddDays(1), time: time);
         _dateTimeProvider.DateOnly.Returns(date);
         _dateTimeProvider.TimeOnly.Returns(time.AddHours(-1));
    
         // Act
         var result = office.BookAppointment(appointment, _dateTimeProvider);
    
         // Assert
         result.IsError.ShouldBeTrue();
         result.FirstError.ShouldBe(OfficeErrors.CannotBookNotAvailableAppointment);
     }

     [Fact]
     public void BookAppointment_Should_AddAppointmentBookedEvent()
     {
         // Arrange
         var office = OfficeFactory.CreateWithSchedule();
         var time = office.WorkingSchedule.Calendar.Last().Value.First().Start;
         var date = office.WorkingSchedule.Calendar.Last().Key;
         var appointment = AppointmentFactory.Create(date: date, time: time);
         _dateTimeProvider.DateOnly.Returns(date.AddDays(-1));
         _dateTimeProvider.TimeOnly.Returns(time);
    
         // Act
         var result = office.BookAppointment(appointment, _dateTimeProvider);
    
         // Assert
         result.IsError.ShouldBeFalse();
         var events = office.PopDomainEvents();
         
         foreach (var @event in events)
         { 
             @event.ShouldBeOfType<AppointmentBookedEvent>();
         }
     }
}