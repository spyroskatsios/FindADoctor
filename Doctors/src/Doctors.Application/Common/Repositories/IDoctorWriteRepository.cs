using Doctors.Domain.DoctorAggregate;

namespace Doctors.Application.Common.Repositories;

public interface IDoctorWriteRepository
{
    Task<Doctor?> GetAsync(DoctorId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default);
}