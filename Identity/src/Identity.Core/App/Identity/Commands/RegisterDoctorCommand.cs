using ErrorOr;
using Identity.Core.App.Common.Authorization;
using Identity.Core.Persistence;
using Identity.Core.Services;
using MediatR;

namespace Identity.Core.App.Identity.Commands;

public record RegisterDoctorCommand(string UserName, string Email, string Password)
    : IRequest<ErrorOr<Guid>>;

public class RegisterDoctorHandler : IRequestHandler<RegisterDoctorCommand, ErrorOr<Guid>>
{
    private readonly IIdentityService _identityService;
    private readonly AppDbContext _dbContext;
    
    public RegisterDoctorHandler(IIdentityService identityService, AppDbContext dbContext)
    {
        _identityService = identityService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Guid>> Handle(RegisterDoctorCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterAsync(request.UserName, request.Email, request.Password, AppRoles.Doctor);
        
        if (result.IsError)
            return result;
            
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return result;
    }
}