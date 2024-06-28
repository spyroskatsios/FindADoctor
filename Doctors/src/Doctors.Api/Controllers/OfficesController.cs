using Doctors.Api.Mapping;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Offices.Commands;
using Doctors.Application.Offices.Queries;
using Doctors.Contracts.Offices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Doctors.Api.Controllers;


public class OfficesController : ApiController
{
    private readonly ISender _mediator;

    public OfficesController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("doctors/{doctorId:guid}/offices")]
    public async Task<IActionResult> Create(CreateOfficeRequest request, Guid doctorId)
    {
        var createOfficeResult = await _mediator.Send(request.ToCreateOfficeCommand(doctorId));
        
        return createOfficeResult.Match(
            office => CreatedAtAction(nameof(GetById), new { officeId = office.Id.Value }, office.ToOfficeResponse()),
            Problem);
    }
    
    
    [HttpGet(("doctors/{doctorId:guid}/offices"))]
    public async Task<IActionResult> Get(Guid doctorId)
    {
        var query = new GetOfficesQuery(doctorId);
        
        var result = await _mediator.Send(query);
        
        return result.Match(
            offices => Ok(offices.ToGetOfficesResponse()),
            Problem);
    }
    
    [HttpGet("offices/{officeId:guid}")]
    public async Task<IActionResult> GetById(Guid officeId)
    {
        var query = new GetOfficeQuery(officeId);
        
        var result = await _mediator.Send(query);
        
        return result.Match(
            office => Ok(office.ToOfficeResponse()),
            Problem);
    }
    
    [HttpDelete("offices/{officeId:guid}")]
    public async Task<IActionResult> DeleteOffice(Guid officeId)
    {
        var command = new DeleteOfficeCommand(officeId);
        
        var result = await _mediator.Send(command);
        
        return result.Match(
            _ => NoContent(),
            Problem);
    }
    
}