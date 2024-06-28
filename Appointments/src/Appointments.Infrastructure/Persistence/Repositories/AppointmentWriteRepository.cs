using Appointments.Application.Common.Repositories;
using Appointments.Domain.AppointmentAggregate;
using Appointments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Infrastructure.Repositories;

public class AppointmentWriteRepository: IAppointmentWriteRepository
{
    private readonly AppDbContext _dbContext;

    public AppointmentWriteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Appointment?> GetAsync(AppointmentId id, CancellationToken cancellationToken = default) 
        => await _dbContext.Appointments.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);

    public async Task CreateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _dbContext.Appointments.Add(appointment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _dbContext.Appointments.Update(appointment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateRangeAsync(IEnumerable<Appointment> appointments, CancellationToken cancellationToken = default)
    {
        foreach (var appointment in appointments)
        {
            _dbContext.Appointments.Update(appointment);
        }
        
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}