using Appointments.Application.Common.Results;
using ErrorOr;

namespace Appointments.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<ErrorOr<Guid>> RegisterAsync(string userName, string email, string password, string role);
    Task<ErrorOr<AuthenticationResult>> LoginAsync(string userName, string password);
    Task<ErrorOr<AuthenticationResult>> RefreshTokenAsync(string token, string refreshToken);
}