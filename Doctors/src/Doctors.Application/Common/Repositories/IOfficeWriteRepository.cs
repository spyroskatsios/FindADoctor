using Doctors.Domain.OfficeAggregate;

namespace Doctors.Application.Common.Repositories;

public interface IOfficeWriteRepository
{
    Task<Office?> GetAsync(OfficeId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Office office, CancellationToken cancellationToken = default);
    Task UpdateAsync(Office office, CancellationToken cancellationToken = default);
}