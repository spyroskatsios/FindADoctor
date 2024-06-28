using ErrorOr;
using Identity.Core.App.Common.Results;
using Identity.Core.Services;
using MediatR;

namespace Identity.Core.App.Identity.Commands;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<ErrorOr<AuthenticationResult>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        => await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
}