using Doctors.Api.Mapping;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Subscriptions.Commands;
using Doctors.Application.Subscriptions.Queries;
using Doctors.Contracts.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionType = Doctors.Domain.SubscriptionAggregate.SubscriptionType;

namespace Doctors.Api.Controllers;

[Route("subscriptions")]
public class SubscriptionsController : ApiController
{
    private readonly ISender _mediator;
    private readonly ICurrentUserService _currentUserService;

    public SubscriptionsController(ISender mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSubscription(CreateSubscriptionRequest request)
    {
        if(!SubscriptionType.TryFromName(request.SubscriptionType.ToString(), out var subscriptionType))
            return Problem("Invalid subscription type", statusCode: StatusCodes.Status400BadRequest);
        
        var command = new CreateSubscriptionCommand(subscriptionType, _currentUserService.DoctorId);
        
        var createSubscriptionResult = await _mediator.Send(command);
        
        return createSubscriptionResult.Match(
            subscription => CreatedAtAction(nameof(GetById), new { subscriptionId = subscription.Id.Value }, subscription.ToSubscriptionResponse()),
            Problem);
    }
    
    [HttpGet("{subscriptionId:guid}")]
    public async Task<IActionResult> GetById(Guid subscriptionId)
    {
        var result = await _mediator.Send(new GetSubscriptionQuery(subscriptionId));
        
        return result.Match(
            subscription => Ok(subscription.ToSubscriptionResponse()),
            Problem);
    }
}