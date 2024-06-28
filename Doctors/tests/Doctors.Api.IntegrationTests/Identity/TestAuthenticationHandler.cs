using System.Security.Claims;
using System.Text.Encodings.Web;
using Doctors.Application.Common.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TestsCommon.TestConstants;

namespace Doctors.Api.IntegrationTests.Identity;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";
    
    public const string RoleClaimType = "role";
    
    public const string Unauthorized = "Unauthorized";
    
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if(Context.Request.Headers.TryGetValue(Unauthorized, out _))
            return Task.FromResult(AuthenticateResult.Fail(Unauthorized));
        
        var claims = new List<Claim> { };
        
        if (Context.Request.Headers.TryGetValue(AppClaims.Id, out var userId))
        {
            claims.Add(new Claim(AppClaims.Id, userId[0]!));
        }
        else
        {
            claims.Add(new Claim(AppClaims.Id, Constants.User.Id));
        }

        if (Context.Request.Headers.TryGetValue(RoleClaimType, out var role))
            claims.Add(new Claim(ClaimTypes.Role, role[0]!));
        
        if(Context.Request.Headers.TryGetValue(AppClaims.DoctorId, out var doctorId))
            claims.Add(new Claim(AppClaims.DoctorId, doctorId[0]!));
        

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    [Obsolete("Obsolete")]
    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
    }

    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) 
        : base(options, logger, encoder)
    {
    }
}