using Doctors.Application.Common.Authorization;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Common.Repositories;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using ErrorOr;
using MediatR;

namespace Doctors.Application.Offices.Commands;

[Authorize(Roles = AppRoles.Doctor)]
public record DeleteOfficeCommand(Guid Id) : IRequest<ErrorOr<Success>>;

public class DeleteOfficeHandler : IRequestHandler<DeleteOfficeCommand, ErrorOr<Success>>
{
    private readonly IDoctorWriteRepository _doctorWriteRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteOfficeHandler(IDoctorWriteRepository doctorWriteRepository, ICurrentUserService currentUserService)
    {
        _doctorWriteRepository = doctorWriteRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorWriteRepository.GetAsync(DoctorId.From(_currentUserService.DoctorId), cancellationToken);
        
        if (doctor is null)
            return Error.NotFound(description: "Doctor not found");
        
        var result = doctor.RemoveOffice(OfficeId.From(request.Id));
        
        if (result.IsError)
            return result;

        await _doctorWriteRepository.UpdateAsync(doctor, cancellationToken);

        return Result.Success;
    }
}