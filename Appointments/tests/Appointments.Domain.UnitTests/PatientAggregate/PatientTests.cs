using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.PatientAggregate;
using TestCommon.Utils.Appointments;
using TestCommon.Utils.Patients;

namespace Appointments.Domain.UnitTests.PatientAggregate;

public class PatientTests
{
    [Fact]
    public void AddAppointment_ShouldFail_WhenAppointmentAlreadyExists()
    {
        // Arrange
        var patient = PatientFactory.Create();
        var appointment = AppointmentFactory.Create();
        patient.AddAppointment(appointment);
        
        // Act
        var result = patient.AddAppointment(appointment);
        
        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(PatientErrors.AppointmentAlreadyExists);
    }
    
    [Fact]
    public void AddAppointment_ShouldFail_WhenOverlappingAppointment()
    {
        // Arrange
        var patient = PatientFactory.Create();
        var appointment = AppointmentFactory.Create();
        var overlappingAppointment = AppointmentFactory.Create(date: appointment.Date, time: appointment.TimeRange.Start.AddMinutes(10), id: AppointmentId.New());
        patient.AddAppointment(appointment);
        
        // Act
        var result = patient.AddAppointment(overlappingAppointment);
        
        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(PatientErrors.AppointmentOverlaps);
    }
}