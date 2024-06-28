using Appointments.Domain.PatientAggregate;

namespace Appointments.Application.Common.Repositories;

public interface IPatientWriteRepository
{
    Task<Patient?> GetAsync(PatientId id, CancellationToken cancellationToken = default);
    Task CreateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
}