using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Appointments.Api.IntegrationTests.Identity;

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