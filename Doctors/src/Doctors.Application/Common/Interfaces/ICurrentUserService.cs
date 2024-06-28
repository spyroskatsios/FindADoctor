using Doctors.Application.Common.Models;

namespace Doctors.Application.Common.Interfaces;

public interface ICurrentUserService
{
    CurrentUser User { get; }
    Guid DoctorId { get; }
    Guid PatientId { get; }
}