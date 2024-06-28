using System.Reflection;
using System.Text;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Common.Repositories;
using Doctors.Domain.Common.Interfaces;
using Doctors.Infrastructure.Events;
using Doctors.Infrastructure.IntegrationEvents;
using Doctors.Infrastructure.Persistence;
using Doctors.Infrastructure.Repositories;
using Doctors.Infrastructure.Services;
using Doctors.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Doctors.Infrastructure;

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
        services.AddHostedService<PublishIntegrationEventsBackgroundService>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        
        services.AddScoped<IReadDbContext, AppDbContext>();

        services.AddScoped<IDoctorWriteRepository, DoctorWriteRepository>();
        services.AddScoped<ISubscriptionWriteRepository, SubscriptionWriteRepository>();
        services.AddScoped<IOfficeWriteRepository, OfficeWriteRepository>();
        
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
        services.AddSingleton<IIntegrationEventsPublisher, IntegrationEventsPublisher>();
        services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
        
        return services;
    }
}