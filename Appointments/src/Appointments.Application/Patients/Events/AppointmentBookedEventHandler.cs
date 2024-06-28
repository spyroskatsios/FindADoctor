using Appointments.Application.Common.Events;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.OfficeAggregate;
using MediatR;

namespace Appointments.Application.Patients.Events;

public class AppointmentBookedEventHandler: INotificationHandler<DomainEventNotification<AppointmentBookedEvent>>
{
    private readonly IPatientWriteRepository _patientWriteRepository;

    public AppointmentBookedEventHandler(IPatientWriteRepository patientWriteRepository)
    {
        _patientWriteRepository = patientWriteRepository;
    }

    public async Task Handle(DomainEventNotification<AppointmentBookedEvent> notification, CancellationToken cancellationToken)
    {
        var patient = await _patientWriteRepository.GetAsync(notification.DomainEvent.Appointment.PatientId, cancellationToken);
        patient!.AddAppointment(notification.DomainEvent.Appointment);
        await _patientWriteRepository.UpdateAsync(patient, cancellationToken);
    }
}