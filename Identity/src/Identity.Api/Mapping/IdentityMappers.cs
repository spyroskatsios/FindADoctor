using Identity.Contracts.Identity;
using Identity.Core.App.Common.Results;
using Identity.Core.App.Identity.Commands;

namespace Identity.Api.Mapping;

public static class IdentityMappers
{
    public static LoginCommand ToLoginCommand(this LoginRequest request) =>
        new(request.UserName, request.Password);
    
    public static IdentityResponse ToIdentityResponse(this AuthenticationResult result) =>
        new(result.Token, result.RefreshToken);
    
    public static RefreshTokenCommand ToRefreshTokenCommand(this RefreshTokenRequest request) =>
        new(request.Token, request.RefreshToken);
}