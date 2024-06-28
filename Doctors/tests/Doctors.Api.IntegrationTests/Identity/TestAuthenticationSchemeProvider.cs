using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Doctors.Api.IntegrationTests.Identity;

public class TestAuthenticationSchemeProvider : AuthenticationSchemeProvider
{
    public TestAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
    {
    }

    protected TestAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options, IDictionary<string, AuthenticationScheme> schemes) : base(options, schemes)
    {
    }

    public override Task<AuthenticationScheme?> GetDefaultAuthenticateSchemeAsync()
    {
        return Task.FromResult(new AuthenticationScheme(TestAuthenticationHandler.AuthenticationScheme, TestAuthenticationHandler.AuthenticationScheme, typeof(TestAuthenticationHandler)))!;
    }
}