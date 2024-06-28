using Doctors.Application.Common.Interfaces;
using Doctors.Application.Common.Repositories;
using Doctors.Domain.DoctorAggregate;
using ErrorOr;
using MediatR;

namespace Doctors.Application.Doctors.Commands;

public record CreateDoctorCommand(string FirstName, string LastName, Speciality Speciality, Guid DoctorId, string UserId) : IRequest<ErrorOr<Doctor>>;
 
 public class CreateDoctorHandler : IRequestHandler<CreateDoctorCommand, ErrorOr<Doctor>>
 {
     private readonly IDoctorWriteRepository _doctorWriteRepository;

     public CreateDoctorHandler(IDoctorWriteRepository doctorWriteRepository, ICurrentUserService currentUserService)
     {
         _doctorWriteRepository = doctorWriteRepository;
     }


     public async Task<ErrorOr<Doctor>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
     {
         var doctor = new Doctor(request.FirstName, request.LastName, request.Speciality, request.UserId, DoctorId.From(request.DoctorId));
         await _doctorWriteRepository.CreateAsync(doctor, cancellationToken);
         return doctor;
     }
 }