using Appointments.Application.Common.Authorization;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.Common.Interfaces;
using Appointments.Domain.OfficeAggregate;
using Appointments.Domain.PatientAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Application.Appointments.Commands;

[Authorize(Roles = AppRoles.Patient)]
public record BookAppointmentCommand(Guid PatientId, Guid OfficeId, DateTime DateTime) : IRequest<ErrorOr<Appointment>>;

public class BookAppointmentHandler : IRequestHandler<BookAppointmentCommand, ErrorOr<Appointment>>
{
    private readonly IOfficeWriteRepository _officeWriteRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IReadDbContext _readDbContext;

    public BookAppointmentHandler(IOfficeWriteRepository officeWriteRepository, IDateTimeProvider dateTimeProvider, IReadDbContext readDbContext)
    {
        _officeWriteRepository = officeWriteRepository;
        _dateTimeProvider = dateTimeProvider;
        _readDbContext = readDbContext;
    }

    public async Task<ErrorOr<Appointment>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeWriteRepository.GetAsync(OfficeId.From(request.OfficeId), cancellationToken);
        
        if(office is null)
            return Error.NotFound(description: "Office not found");
        
        var patient = await _readDbContext.Patients.FirstOrDefaultAsync(x=>x.Id == PatientId.From(request.PatientId), cancellationToken);
        
        if(patient is null)
            return Error.NotFound(description: "Patient not found");
        
        var appointment = new Appointment(office.Id, patient.Id, office.DoctorId,
            DateOnly.FromDateTime(request.DateTime), TimeOnly.FromDateTime(request.DateTime));

        if(patient.HasAppointment(appointment.Id))
            return PatientErrors.AppointmentAlreadyExists;
        
        if (patient.HasOverlappingAppointment(appointment.Date, appointment.TimeRange))
            return PatientErrors.AppointmentOverlaps;
        
        var result = office.BookAppointment(appointment, _dateTimeProvider);

        if (result.IsError)
            return result.Errors;
        
        await _officeWriteRepository.UpdateAsync(office, cancellationToken);

        return appointment;
    }
}