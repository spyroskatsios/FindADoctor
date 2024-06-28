namespace Appointments.Contracts.Appointments;

public record AppointmentResponse(Guid Id, Guid OfficeId, Guid PatientId, Guid DoctorId, DateTime Start, DateTime End, AppointmentStatus Status);

