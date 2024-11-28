using Appointments.Api.Installers;
using Appointments.Api.Services;
using Appointments.Application.Common.Interfaces;
using Appointments.Infrastructure.Persistence;

namespace Appointments.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddHttpContextAccessor();
        services.AddAuthentication();
        services.AddExceptionHandling();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.AddOpenTelemetry(configuration);

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        return services;
    }

}