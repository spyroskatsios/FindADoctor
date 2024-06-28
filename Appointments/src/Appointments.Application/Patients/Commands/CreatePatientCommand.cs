using Appointments.Application.Common.Authorization;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.PatientAggregate;
using ErrorOr;
using MediatR;

namespace Appointments.Application.Patients.Commands;

[Authorize(Roles = AppRoles.Patient)]
public record CreatePatientCommand(string UserId, Guid PatientId) : IRequest<ErrorOr<Patient>>;

public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, ErrorOr<Patient>>
{
    private readonly IPatientWriteRepository _patientWriteRepository;

    public CreatePatientHandler(IPatientWriteRepository patientWriteRepository)
    {
        _patientWriteRepository = patientWriteRepository;
    }

    public async Task<ErrorOr<Patient>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient(request.UserId, PatientId.From(request.PatientId));
        await _patientWriteRepository.CreateAsync(patient, cancellationToken);
        return patient;
    }
}