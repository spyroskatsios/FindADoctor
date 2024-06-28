using Identity.Api.Services;
using Identity.Core.Services;

namespace Identity.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddHttpContextAccessor();
        services.AddAuthentication();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

}