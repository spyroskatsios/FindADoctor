using Appointments.Application.Common.Repositories;
using Appointments.Domain.PatientAggregate;
using Appointments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Infrastructure.Repositories;

public class PatientWriteRepository : IPatientWriteRepository
{
private readonly AppDbContext _dbContext;

public PatientWriteRepository(AppDbContext dbContext)
{
    _dbContext = dbContext;
}


public async Task<Patient?> GetAsync(PatientId id, CancellationToken cancellationToken = default) 
    => await _dbContext.Patients.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);

public async Task CreateAsync(Patient patient, CancellationToken cancellationToken = default)
{
    _dbContext.Patients.Add(patient);
    await _dbContext.SaveChangesAsync(cancellationToken);
}

public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
{
    _dbContext.Patients.Update(patient);
    await _dbContext.SaveChangesAsync(cancellationToken);
}
}