using System.Reflection;
using System.Text;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.Common.Interfaces;
using Appointments.Infrastructure.Events;
using Appointments.Infrastructure.IntegrationEvents;
using Appointments.Infrastructure.Persistence;
using Appointments.Infrastructure.Persistence.Repositories;
using Appointments.Infrastructure.Repositories;
using Appointments.Infrastructure.Services;
using Appointments.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Appointments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration)
            .AddIdentity(configuration)
            .AddSettings(configuration)
            .AddBackgroundServices()
            .AddServices()
            .AddMediatR();
        
        return services;
    }
    
    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        //services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentIntegrationEventHandler<>));
        return services;
    }
    
    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection(RabbitMQSettings.Section));
        return services;
    }
    
    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<ConsumeIntegrationEventsBackgroundService>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        
        services.AddScoped<IReadDbContext, AppDbContext>();

        services.AddScoped<IOfficeWriteRepository, OfficeWriteRepository>();
        services.AddScoped<IPatientWriteRepository, PatientWriteRepository>();
        services.AddScoped<IAppointmentWriteRepository, AppointmentWriteRepository>();
        
        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings(); 
        configuration.Bind(JwtSettings.Section, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret))
            });
        
        
        return services;
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
        
        return services;
    }
}