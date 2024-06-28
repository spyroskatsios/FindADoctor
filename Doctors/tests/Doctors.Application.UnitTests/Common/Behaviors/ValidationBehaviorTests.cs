using Doctors.Application.Common.Behaviors;
using Doctors.Application.Doctors.Commands;
using Doctors.Application.Offices.Commands;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using TestsCommon.Utils.Doctors;
using TestsCommon.Utils.Offices;

namespace Doctors.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateOfficeCommand, ErrorOr<Office>> _sut;
    private readonly IValidator<CreateOfficeCommand> _validator;
    private readonly RequestHandlerDelegate<ErrorOr<Office>> _next;

    public ValidationBehaviorTests()
    {
        _validator = Substitute.For<IValidator<CreateOfficeCommand>>();
        _sut = new ValidationBehavior<CreateOfficeCommand, ErrorOr<Office>>(new []{_validator});
        _next = Substitute.For<RequestHandlerDelegate<ErrorOr<Office>>>();
    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var command = OfficeCommandFactory.CreateCreateOfficeCommand();
        
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
        var command = OfficeCommandFactory.CreateCreateOfficeCommand();
        
        const string propertyName = "MyProperty";
        const string errorMessage = "MyProperty is invalid";
        
        var validationFailures = new List<ValidationFailure>{new (propertyName, errorMessage)};

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _sut.Handle(command, _next, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be(propertyName);
        result.FirstError.Description.Should().Be(errorMessage);
    }
}