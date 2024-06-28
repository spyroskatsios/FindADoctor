using Doctors.Application.Common.Interfaces;
using Doctors.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Application.Subscriptions.Queries;

public record GetSubscriptionQuery(Guid SubscriptionId) : IRequest<ErrorOr<Subscription>>;

public class GetSubscriptionQueryHandler : IRequestHandler<GetSubscriptionQuery, ErrorOr<Subscription>>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetSubscriptionQueryHandler(IReadDbContext readDbContext, ICurrentUserService currentUserService)
    {
        _readDbContext = readDbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Subscription>> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _readDbContext.Subscriptions.AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == SubscriptionId.From(request.SubscriptionId), cancellationToken);
        
        if (subscription is null)
            return Error.NotFound(description: "Subscription not found");

        if (subscription.DoctorId.Value != _currentUserService.DoctorId)
            return Error.NotFound();
        
        return subscription;
    }
}