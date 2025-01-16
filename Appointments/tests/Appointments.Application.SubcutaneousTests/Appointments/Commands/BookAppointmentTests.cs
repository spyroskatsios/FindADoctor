using ErrorOr;
using MediatR;
using TestCommon.Utils.Appointments;
using TestCommon.Utils.Common;
using TestCommon.Utils.Offices;
using TestCommon.Utils.Patients;

namespace Appointments.Application.SubcutaneousTests.Appointments.Commands;

[Collection(TestCollection.Name)]
public class BookAppointmentTests
{
    private readonly AppointmentsFactory _appointmentsFactory;
    private readonly IMediator _mediator;
    
    public BookAppointmentTests(AppointmentsFactory appointmentsFactory)
    {
        _appointmentsFactory = appointmentsFactory;
        _mediator = _appointmentsFactory.GetMediator();
    }
    
    [Fact]
    public async Task CreateOffice_WhenValidCommand_ShouldCreateOffice()
    {
        // Arrange
        _appointmentsFactory.SetCurrentUsePatient();
        var office = OfficeFactory.CreateWithSchedule();
        _appointmentsFactory.CreateOffice(office);
        _appointmentsFactory.CreatePatient(PatientFactory.Create());
        var date = office.WorkingSchedule.Calendar.First().Key;
        var time = office.WorkingSchedule.Calendar.First().Value.First().Start;
        var command = AppointmentCommandFactory.CreateBookAppointmentCommand(dateTime: date.ToDateTime(time));
        
        // Act
        var result = await _mediator.Send(command);
        
        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.PatientId.Value.ShouldBe(command.PatientId);
    }
    
    [Fact]
    public async Task CreateOffice_WhenInValidCommand_ShouldReturnValidationError()
    {
        // Arrange
        _appointmentsFactory.SetCurrentUsePatient();
        _appointmentsFactory.CreateOffice(OfficeFactory.Create());
        var command = AppointmentCommandFactory.CreateBookAppointmentCommand(dateTime: DateTime.UtcNow.AddHours(-1));
        
        // Act
        var result = await _mediator.Send(command);
        
        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Validation);
        result.FirstError.Code.ShouldBe("DateTime");
    }
}