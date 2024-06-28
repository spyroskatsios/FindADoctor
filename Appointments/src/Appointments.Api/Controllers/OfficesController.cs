using Appointments.Api.Mapping;
using Appointments.Application.Offices.Queries;
using Appointments.Contracts.Schedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Api.Controllers;


public class OfficesController : ApiController
{
    private readonly ISender _mediator;

    public OfficesController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    
    [HttpPost("offices/{officeId:guid}/schedule")]
    public async Task<IActionResult> CreateSchedule(Guid officeId, AddScheduleRequest request)
    {
        var result = await _mediator.Send(request.ToAddScheduleCommand(officeId));
        
        return result.Match(
            _ => Ok(),
            Problem);
    }
    
    [HttpGet("offices/{officeId:guid}/schedule")]
    public async Task<IActionResult> GetSchedule(Guid officeId)
    {
        var query = new GetScheduleQuery(officeId);
        
        var result = await _mediator.Send(query);
        
        return result.Match(
            schedule => Ok(schedule.ToScheduleResponse()),
            Problem);
    }
    
    
}