using Identity.Api.Mapping;
using Identity.Contracts.Identity;
using Identity.Core.App.Identity.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("identity")]
[AllowAnonymous]
public class IdentityController : ApiController
{
    private readonly ISender _mediator;

    public IdentityController(ISender mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("register-doctor")]
    public async Task<IActionResult> RegisterDoctor(RegisterDoctorRequest request)
    {
        var result = await _mediator.Send(
            new RegisterDoctorCommand(request.UserName, request.Email, request.Password));

        return result.Match<IActionResult>(
            id => Ok(new {DoctorId = id}),
            Problem
        );
    }
    
    [Authorize]
    [HttpPost("register-patient")]
    public async Task<IActionResult> RegisterPatient(RegisterPatientRequest request)
    {
        var result = await _mediator.Send(
            new RegisterPatientCommand(request.UserName, request.Email, request.Password));

        return result.Match<IActionResult>(
            id => Ok(new {PatientId = id}),
            Problem
        );
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var result = await _mediator.Send(new LoginCommand(loginRequest.UserName, loginRequest.Password));

        return result.Match(
            success => Ok(success.ToIdentityResponse()),
            Problem
        );
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.Token, request.RefreshToken));

        return result.Match(
            success => Ok(success.ToIdentityResponse()),
            Problem
        );
    }
    
}