using Appointments.Application.Common.Repositories;
using Appointments.Domain.OfficeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Infrastructure.Persistence.Repositories;

public class OfficeWriteRepository : IOfficeWriteRepository
{
    private readonly AppDbContext _dbContext;

    public OfficeWriteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Office?> GetAsync(OfficeId id, CancellationToken cancellationToken = default) 
        => await _dbContext.Offices.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);

    public async Task CreateAsync(Office office, CancellationToken cancellationToken = default)
    {
        _dbContext.Offices.Add(office);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Office office, CancellationToken cancellationToken = default)
    {
        _dbContext.Offices.Update(office);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}