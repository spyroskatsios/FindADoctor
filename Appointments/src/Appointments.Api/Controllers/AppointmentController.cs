using Appointments.Api.Mapping;
using Appointments.Application.Appointments.Commands;
using Appointments.Application.Appointments.Queries;
using Appointments.Application.Common.Interfaces;
using Appointments.Contracts.Appointments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Api.Controllers;

public class AppointmentController : ApiController
{
    private readonly ISender _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AppointmentController(ISender mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpPost("offices/{officeId:guid}/appointments/")]
    public async Task<IActionResult> Book(BookAppointmentRequest request, Guid officeId)
    {
        var result = await _mediator.Send(request.ToBookAppointmentCommand(officeId, _currentUserService.PatientId));
        return result.Match(
            appointment => CreatedAtAction(nameof(Get), new { appointmentId = appointment.Id.Value}, appointment.ToAppointmentResponse()),
            Problem);
    }
    
    [HttpGet("appointments/{appointmentId:guid}")]
    public async Task<IActionResult> Get(Guid appointmentId)
    {
        var result = await _mediator.Send(new GetAppointmentQuery(appointmentId));
        return result.Match(
            appointment => Ok(appointment.ToAppointmentResponse()),
            Problem);
    }
    
    [HttpPost("appointments/{appointmentId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid appointmentId)
    {
        var result = await _mediator.Send(new CancelAppointmentCommand(appointmentId));
        return result.Match(
            _ => NoContent(),
            Problem);
    }
}