using Doctors.Application.Common.Repositories;
using Doctors.Domain.DoctorAggregate;
using Doctors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Infrastructure.Repositories;

public class DoctorWriteRepository : IDoctorWriteRepository
{
    private readonly AppDbContext _dbContext;

    public DoctorWriteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Doctor?> GetAsync(DoctorId id, CancellationToken cancellationToken = default) 
        => await _dbContext.Doctors.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);

    public async Task CreateAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _dbContext.Doctors.Add(doctor);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _dbContext.Doctors.Update(doctor);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}