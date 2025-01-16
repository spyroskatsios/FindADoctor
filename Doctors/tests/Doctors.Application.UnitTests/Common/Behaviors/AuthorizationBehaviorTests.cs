using Doctors.Application.Common.Authorization;
using Doctors.Application.Common.Behaviors;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Common.Models;
using Doctors.Application.Common.Results;
using Doctors.Application.Offices.Commands;
using Doctors.Domain.OfficeAggregate;
using ErrorOr;
using MediatR;
using TestsCommon.TestConstants;
using TestsCommon.Utils.Common;
using TestsCommon.Utils.Offices;

namespace Doctors.Application.UnitTests.Common.Behaviors;

public class AuthorizationBehaviorTests
{
    
    [Fact]
    public async Task InvokeBehavior_WhenRoleAuthorizedAndUserHasNotRole_ShouldReturnUnauthorized()
    {
        // Arrange
        var currentUserService = Substitute.For<ICurrentUserService>();
        var sut = new AuthorizationBehavior<DeleteOfficeCommand, ErrorOr<Success>>(currentUserService);
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Success>>>();
        var command = OfficeCommandFactory.CreateDeleteOfficeCommand();
        currentUserService.User.Returns(new CurrentUser(Constants.User.Id,new List<string>(), new List<string>()));
        
        // Act
        var result = await sut.Handle(command, next, default);
        
        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Forbidden);

    }
    
    [Fact]
    public async Task InvokeBehavior_WhenRoleAuthorizedAndUserHasRole_ShouldInvokeNextBehavior()
    {
        // Arrange
        var currentUserService = Substitute.For<ICurrentUserService>();
        var sut = new AuthorizationBehavior<DeleteOfficeCommand, ErrorOr<Success>>(currentUserService);
        var next = Substitute.For<RequestHandlerDelegate<ErrorOr<Success>>>();
        var command = OfficeCommandFactory.CreateDeleteOfficeCommand();
        var currentUser = CurrentUserFactory.CreateCurrentUser(roles: new []{AppRoles.Doctor});
        currentUserService.User.Returns(currentUser);
        
        // Act
        await sut.Handle(command, next, default);
        
        // Assert
        await next.Received(1).Invoke();

    }
}