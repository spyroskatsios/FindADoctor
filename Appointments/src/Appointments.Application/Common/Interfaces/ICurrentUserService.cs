using Appointments.Application.Common.Models;

namespace Appointments.Application.Common.Interfaces;

public interface ICurrentUserService
{
    CurrentUser User { get; }
    Guid DoctorId { get; }
    Guid PatientId { get; }
}