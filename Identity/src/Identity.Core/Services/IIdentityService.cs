using ErrorOr;
using Identity.Core.App.Common.Results;

namespace Identity.Core.Services;

public interface IIdentityService
{
    Task<ErrorOr<Guid>> RegisterAsync(string userName, string email, string password, string role);
    Task<ErrorOr<AuthenticationResult>> LoginAsync(string userName, string password);
    Task<ErrorOr<AuthenticationResult>> RefreshTokenAsync(string token, string refreshToken);
}