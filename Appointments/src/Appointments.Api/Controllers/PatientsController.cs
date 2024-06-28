using Appointments.Api.Mapping;
using Appointments.Application.Common.Authorization;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Patients.Commands;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Api.Controllers;

[Route("patients")]
public class PatientsController : ApiController
{
    private readonly ISender _mediator;
    private readonly ICurrentUserService _currentUserService;

    public PatientsController(ISender mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> Create()
    {
        var result = await _mediator.Send(new CreatePatientCommand(_currentUserService.User.Id, _currentUserService.PatientId));
        return result.Match(
            patient => Ok(patient.ToPatientResponse()),
            Problem);
    }
}