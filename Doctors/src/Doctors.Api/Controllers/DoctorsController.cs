using Doctors.Api.Mapping;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Doctors.Commands;
using Doctors.Application.Doctors.Queries;
using Doctors.Contracts.Doctors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Speciality = Doctors.Domain.DoctorAggregate.Speciality;

namespace Doctors.Api.Controllers;

[Route("doctors")]
public class DoctorsController : ApiController
{
    private readonly ISender _mediator;
    private readonly ICurrentUserService _currentUserService;

    public DoctorsController(ISender mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateDoctorRequest request)
    {
        if(!Speciality.TryFromName(request.Speciality.ToString(), out var speciality))
            return Problem("Invalid speciality", statusCode: StatusCodes.Status400BadRequest);
        
        var command = new CreateDoctorCommand(request.FirstName, request.LastName, speciality, _currentUserService.DoctorId, _currentUserService.User.Id);

        var result = await _mediator.Send(command);

        return result.Match(
            doctor => CreatedAtAction(nameof(GetById), new { doctorId = doctor.Id.Value }, doctor.ToDoctorResponse()),
            Problem);
    }

    [HttpGet("{doctorId:guid}")]
    public async Task<IActionResult> GetById(Guid doctorId)
    {
        var result = await _mediator.Send(new GetDoctorQuery(doctorId));

        return result.Match(
            doctor => Ok(doctor.ToDoctorResponse()),
            Problem);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchDoctorsRequest request)
    {
        var result = await _mediator.Send(request.ToSearchDoctorsQuery());
        
        return result.Match(
            doctors => Ok(doctors.ToPagedResponse(x=>x.ToDoctorResponse())),
            Problem);
    }
}