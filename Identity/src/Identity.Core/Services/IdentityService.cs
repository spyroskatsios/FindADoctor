using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ErrorOr;
using Identity.Core.App.Common.Authorization;
using Identity.Core.App.Common.Results;
using Identity.Core.Entities;
using Identity.Core.Persistence;
using Identity.Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Identity.Core.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext, 
        IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _jwtSettings = jwtOptions.Value;
    }
    
    public async Task<ErrorOr<Guid>> RegisterAsync(string userName, string email, string password, string role)
    {
        var roleId = Guid.NewGuid();
        
        var user = new AppUser
        {
            UserName = userName,
            Email = email,
            DoctorId = role == AppRoles.Doctor ? roleId : Guid.Empty,
            PatientId = role == AppRoles.Patient ? roleId : Guid.Empty
        };
            
        
        var result = await _userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)
            return result.Errors.Select(x=>Error.Failure(description: x.Description)).ToList();
        
        if (!await _roleManager.RoleExistsAsync(role))
            return Error.Validation(description: "Role does not exist");

        result = await _userManager.AddToRolesAsync(user, new []{role});
        
        if (!result.Succeeded)
            return result.Errors.Select(x=>Error.Failure(description: x.Description)).ToList();

        return roleId;
    }
    
    public async Task<ErrorOr<AuthenticationResult>> LoginAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        
        if (user is null)
            return Error.Validation(description: "Username or password is incorrect");
        
        var result = await _userManager.CheckPasswordAsync(user, password);
        
        if (!result)
            return Error.Validation(description: "Username or password is incorrect");

        return await GenerateAuthenticationResultAsync(user);
    }
    
    
    public async Task<ErrorOr<AuthenticationResult>> RefreshTokenAsync(string token, string refreshToken)
    {
        var result = GetPrincipalFromToken(token);

        if (result.IsError)
            return Error.Unauthorized();
        
        var validatedToken = result.Value;
        
        var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
        
        var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix);
        
        if (expiryDateTimeUtc < _dateTimeProvider.UtcNow)
            return Error.Unauthorized();
        
        var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        
        var storedRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Value == refreshToken);

        if (storedRefreshToken is null || storedRefreshToken.Invalidated || DateTime.UtcNow > storedRefreshToken.ExpirationDateTime
            || storedRefreshToken.Used || storedRefreshToken.JwtId != jti)
            return Error.Unauthorized();
        
        var userId = validatedToken.Claims.Single(x => x.Type == "id").Value;
        
        var user = await _userManager.FindByIdAsync(userId);
        
        storedRefreshToken.Used = true;

        return await GenerateAuthenticationResultAsync(user!);
    }

    private async Task<AuthenticationResult> GenerateAuthenticationResultAsync(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(AppClaims.Id, user.Id),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var roles = await _userManager.GetRolesAsync(user);
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        AddIds(user, claims);
        
        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
            signingCredentials: credentials
        );

        var refreshToken = new RefreshToken
        {
            Value = GenerateRefreshTokenValue(),
            CreationDateTime = DateTime.UtcNow,
            ExpirationDateTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtSettings.RefreshTokenExpirationInMinutes)),
            JwtId = token.Id,
            UserId = user.Id
        };
        
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new AuthenticationResult(new JwtSecurityTokenHandler().WriteToken(token), refreshToken.Value);
    }
    
    private ErrorOr<ClaimsPrincipal> GetPrincipalFromToken(string token)
    {
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            ClaimsPrincipal principal;

            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            }
            catch (Exception)
            {
                return Error.Unauthorized();
            }

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                return Error.Unauthorized();
            

            return principal;
        }
    }
    
    private static void AddIds(AppUser user, List<Claim> claims)
        => claims
            .AddIfNotEmpty(AppClaims.DoctorId, user.DoctorId)
            .AddIfNotEmpty(AppClaims.PatientId, user.PatientId);
    
    private static string GenerateRefreshTokenValue()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}