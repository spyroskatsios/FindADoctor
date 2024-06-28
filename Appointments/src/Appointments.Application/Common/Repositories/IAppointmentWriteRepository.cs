using Appointments.Domain.AppointmentAggregate;

namespace Appointments.Application.Common.Repositories;

public interface IAppointmentWriteRepository
{
    Task<Appointment?> GetAsync(AppointmentId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<Appointment> appointments, CancellationToken cancellationToken = default);
    
}