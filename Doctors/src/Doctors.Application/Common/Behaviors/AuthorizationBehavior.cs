using System.Reflection;
using Doctors.Application.Common.Authorization;
using Doctors.Application.Common.Interfaces;
using ErrorOr;
using MediatR;

namespace Doctors.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationBehavior(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizationAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();
        
        if (authorizationAttributes.Count == 0)
            return await next();
        
        var permissions = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Permissions?.Split(',') ?? [])
            .ToList();
        
        if(permissions.Except(_currentUserService.User.Permissions).Any())
            return (dynamic)Error.Forbidden();
        
        var roles = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? [])
            .ToList();

        if (roles.Except(_currentUserService.User.Roles).Any())
            return (dynamic)Error.Forbidden();

        return await next();
        
    }
}