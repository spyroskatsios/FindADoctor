using Appointments.Application.Appointments.Commands;
using Appointments.Application.Common.Behaviors;
using Appointments.Domain.AppointmentAggregate;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using TestCommon.TestConstants;
using TestCommon.Utils.Appointments;
using TestCommon.Utils.Offices;

namespace Appointments.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<BookAppointmentCommand, ErrorOr<Appointment>> _sut;
    private readonly IValidator<BookAppointmentCommand> _validator;
    private readonly RequestHandlerDelegate<ErrorOr<Appointment>> _next;

    public ValidationBehaviorTests()
    {
        _validator = Substitute.For<IValidator<BookAppointmentCommand>>();
        _sut = new ValidationBehavior<BookAppointmentCommand, ErrorOr<Appointment>>(new []{_validator});
        _next = Substitute.For<RequestHandlerDelegate<ErrorOr<Appointment>>>();
    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var command = AppointmentCommandFactory.CreateBookAppointmentCommand();
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        

        // Act
        await _sut.Handle(command, _next, default);

        // Assert
        _next.Received(1);
    }
    
    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
    {
        // Arrange
        var command = AppointmentCommandFactory.CreateBookAppointmentCommand();
        
        const string propertyName = "MyProperty";
        const string errorMessage = "MyProperty is invalid";
        
        var validationFailures = new List<ValidationFailure>{new (propertyName, errorMessage)};

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, _next, default);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Validation);
        result.FirstError.Code.ShouldBe(propertyName);
        result.FirstError.Description.ShouldBe(errorMessage);
    }
}