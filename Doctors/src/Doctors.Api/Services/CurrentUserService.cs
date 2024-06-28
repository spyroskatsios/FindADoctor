using System.Security.Claims;
using Doctors.Application.Common.Authorization;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Common.Models;
using Throw;

namespace Doctors.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUser User { get; }
    public Guid DoctorId => User.Roles.Contains(AppRoles.Doctor) ? Guid.Parse(GetClaimValues(AppClaims.DoctorId).First()) : Guid.Empty;
    public Guid PatientId => User.Roles.Contains(AppRoles.Patient) ? Guid.Parse(GetClaimValues(AppClaims.PatientId).First()) : Guid.Empty;
    
    
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        _httpContextAccessor.HttpContext.ThrowIfNull();
        
        var id = GetClaimValues("id")
            .FirstOrDefault();

        var permissions = GetClaimValues("permissions");
        var roles = GetClaimValues(ClaimTypes.Role);

        User = new CurrentUser(id ?? string.Empty,  permissions, roles);
    }
    
    private IEnumerable<string> GetClaimValues(string claimType) 
        => _httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();
}