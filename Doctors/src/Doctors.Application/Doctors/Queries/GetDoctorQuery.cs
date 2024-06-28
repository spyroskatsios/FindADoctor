using Doctors.Application.Common.Interfaces;
using Doctors.Domain.DoctorAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Application.Doctors.Queries;

public record GetDoctorQuery(Guid DoctorId)
    : IRequest<ErrorOr<Doctor>>;
    

public class GetDoctorQueryHandler : IRequestHandler<GetDoctorQuery, ErrorOr<Doctor>>
{
    private readonly IReadDbContext _readDbContext;

    public GetDoctorQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }


    public async Task<ErrorOr<Doctor>> Handle(GetDoctorQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _readDbContext.Doctors.AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id.Value == request.DoctorId, cancellationToken);
        
        if(doctor is null)
            return Error.NotFound(description: "Doctor not found");
       
        return doctor;
    }
}