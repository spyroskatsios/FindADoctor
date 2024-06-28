using Identity.Core.App.Common.Models;

namespace Identity.Core.Services;

public interface ICurrentUserService
{
    CurrentUser User { get; }
    Guid DoctorId { get; }
    Guid PatientId { get; }
}