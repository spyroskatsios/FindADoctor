using Doctors.Application.Common.Repositories;
using Doctors.Domain.SubscriptionAggregate;
using Doctors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Infrastructure.Repositories;

public class SubscriptionWriteRepository: ISubscriptionWriteRepository
{
    private readonly AppDbContext _dbContext;

    public SubscriptionWriteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Subscription?> GetAsync(SubscriptionId id, CancellationToken cancellationToken = default) 
        => await _dbContext.Subscriptions.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);

    public async Task CreateAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        _dbContext.Subscriptions.Add(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        _dbContext.Subscriptions.Update(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}