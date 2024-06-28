﻿using ErrorOr;
using MediatR;
using TestsCommon.Utils.Doctors;
using TestsCommon.Utils.Offices;

namespace Doctors.Application.SubcutaneousTests.Offices.Commands;

[Collection(TestCollection.Name)]
public class CreateOfficeTests
{
    private readonly DoctorsApiFactory _doctorsApiFactory;
    private readonly IMediator _mediator;
    
    public CreateOfficeTests(DoctorsApiFactory doctorsApiFactory)
    {
        _doctorsApiFactory = doctorsApiFactory;
        _mediator = _doctorsApiFactory.GetMediator();
    }
    
    [Fact]
    public async Task CreateOffice_WhenValidCommand_ShouldCreateOffice()
    {
        // Arrange
        _doctorsApiFactory.SetCurrentUserDoctor();
        _doctorsApiFactory.CreateDoctor(DoctorFactory.Create());
        var command = OfficeCommandFactory.CreateCreateOfficeCommand();
        
        // Act
        var result = await _mediator.Send(command);
        
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.DoctorId.Value.Should().Be(command.DoctorId);
    }
    
    [Fact]
    public async Task CreateOffice_WhenInValidCommand_ShouldReturnValidationError()
    {
        // Arrange
        _doctorsApiFactory.SetCurrentUserDoctor();
        _doctorsApiFactory.CreateDoctor(DoctorFactory.Create());
        var command = OfficeCommandFactory.CreateCreateOfficeCommand(state: "a");
        
        // Act
        var result = await _mediator.Send(command);
        
        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("State");
    }
}