using Appointments.Application.Appointments.Commands;
using Appointments.Application.Common.Authorization;
using Appointments.Application.Common.Behaviors;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Common.Models;
using Appointments.Domain.AppointmentAggregate;
using ErrorOr;
using MediatR;
using TestCommon.TestConstants;
using TestCommon.Utils.Appointments;
using TestCommon.Utils.Common;

namespace Appointments.Application.UnitTests.Common.Behaviors;

public class AuthorizationBehaviorTests
{
    
    [Fact]
    public async Task InvokeBehavior_WhenRoleAuthorizedAndUserHasNotRole_ShouldReturnUnauthorized()
    {
        // Arrange
        var currentUserService = Substitute.For<ICurrentUserService>();
        var sut = new AuthorizationBehavior<BookAppointmentCommand, ErrorOr<Appointment>>(currentUserService);
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Appointment>>>();
        var command = AppointmentCommandFactory.CreateBookAppointmentCommand();
        currentUserService.User.Returns(new CurrentUser(Constants.User.Id,new List<string>(), new List<string>()));
        
        // Act
        var result = await sut.Handle(command, next, default);
        
        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Forbidden);

    }
    
    [Fact]
    public async Task InvokeBehavior_WhenRoleAuthorizedAndUserHasRole_ShouldInvokeNextBehavior()
    {
        // Arrange
        var currentUserService = Substitute.For<ICurrentUserService>();
        var sut = new AuthorizationBehavior<BookAppointmentCommand, ErrorOr<Appointment>>(currentUserService);
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Appointment>>>();
        var command = AppointmentCommandFactory.CreateBookAppointmentCommand();
        var currentUser = CurrentUserFactory.CreateCurrentUser(roles: new []{AppRoles.Doctor});
        currentUserService.User.Returns(currentUser);
        
        // Act
        await sut.Handle(command, next, default);
        
        // Assert
        next.Received(1);

    }
}