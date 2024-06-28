using Doctors.Domain.SubscriptionAggregate;

namespace Doctors.Application.Common.Repositories;

public interface ISubscriptionWriteRepository
{
    Task<Subscription?> GetAsync(SubscriptionId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default);
}