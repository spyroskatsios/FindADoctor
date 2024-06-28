using ErrorOr;
using Identity.Core.App.Common.Results;
using Identity.Core.Services;
using MediatR;

namespace Identity.Core.App.Identity.Commands;

public record LoginCommand(string UserName, string Password) : IRequest<ErrorOr<AuthenticationResult>>;

public class LoginHandler : IRequestHandler<LoginCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IIdentityService _identityService;

    public LoginHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken) 
        => await _identityService.LoginAsync(request.UserName, request.Password);
}