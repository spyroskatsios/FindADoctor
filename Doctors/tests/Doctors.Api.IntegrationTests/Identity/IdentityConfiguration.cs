using System.Text;
using Doctors.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Doctors.Api.IntegrationTests.Identity;

public static class IdentityConfiguration
{

   public static IServiceCollection AddTestIdentity(this IServiceCollection services)
   {
       services.AddSingleton<IAuthenticationSchemeProvider, TestAuthenticationSchemeProvider>();
       
       services.AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
           .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.AuthenticationScheme, _ => { });
           
       return services;
   }
}