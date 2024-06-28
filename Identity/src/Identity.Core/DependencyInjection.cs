using System.Reflection;
using System.Text;
using Identity.Core.App.Common.Behaviors;
using Identity.Core.Entities;
using Identity.Core.Persistence;
using Identity.Core.Services;
using Identity.Core.Settings;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Core;


public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration)
            .AddIdentity(configuration)
            .AddOptions()
            .AddServices()
            .AddSettings(configuration)
            .AddMediatR();
        
        return services;
    }
    
    
    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly(); 
        
        services.AddMediatR(options => options.RegisterServicesFromAssembly(assembly));
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
    
    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection(RabbitMQSettings.Section));
        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        
        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUser, IdentityRole>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = true;
            o.Password.RequireUppercase = true;
            o.Password.RequireNonAlphanumeric = true;
            o.Password.RequiredLength = 6;
            o.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        
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
        
        services.AddScoped<IIdentityService, IdentityService>();
        
        return services;
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        return services;
    }
}