using Doctors.Api.Installers;
using Doctors.Api.Services;
using Doctors.Application.Common.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Doctors.Api;

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

        return services;
    }
    
}