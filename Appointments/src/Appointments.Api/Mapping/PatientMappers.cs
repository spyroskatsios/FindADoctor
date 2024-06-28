using Appointments.Application.Patients.Commands;
using Appointments.Contracts.Patients;
using Appointments.Domain.PatientAggregate;

namespace Appointments.Api.Mapping;

public static class PatientMappers
{
    public static PatientResponse ToPatientResponse(this Patient patient)
        => new(patient.Id.Value);
}