using Doctors.Application.Common.Authorization;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Common.Repositories;
using Doctors.Domain.Common.ValueObjects;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using ErrorOr;
using MediatR;

namespace Doctors.Application.Offices.Commands;

[Authorize(Roles = AppRoles.Doctor)]
public record CreateOfficeCommand(string State, string City, string Street, string StreetNumber, string ZipCode, Guid DoctorId) : IRequest<ErrorOr<Office>>;

public class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, ErrorOr<Office>>
{
    private readonly IDoctorWriteRepository _doctorWriteRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateOfficeCommandHandler(IDoctorWriteRepository doctorWriteRepository, ICurrentUserService currentUserService)
    {
        _doctorWriteRepository = doctorWriteRepository;
        _currentUserService = currentUserService;
    }


    public async Task<ErrorOr<Office>> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorWriteRepository.GetAsync(DoctorId.From(request.DoctorId), cancellationToken);
        
        if(doctor is null)
            return Error.NotFound(description: "Doctor not found");
        
        if(doctor.Id.Value != _currentUserService.DoctorId)
            return Error.NotFound(description: "Doctor not found");

        var address = new Address(request.State, request.City, request.Street, request.StreetNumber, request.ZipCode);
        
        var office = new Office(address, doctor.Id);
       
        var result = doctor.AddOffice(office);
        
        if (result.IsError)
            return result.Errors;

        await _doctorWriteRepository.UpdateAsync(doctor, cancellationToken);
        
        return office;
    }
}