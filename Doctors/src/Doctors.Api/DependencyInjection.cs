using System.Reflection;
using Doctors.Api.Diagnostics;
using Doctors.Api.Services;
using Doctors.Application.Common.Interfaces;
using Doctors.Infrastructure.Settings;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.AddOpenTelemetry(configuration);

        return services;
    }

    private static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var otlpEndpoint = new Uri(configuration.GetValue<string>("OTLP_Endpoint")!);
        
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddService("Doctors",
                        "FindADoctor",
                        Assembly.GetExecutingAssembly().GetName().Version!.ToString());
            })
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddSource(RabbitMqDiagnostics.ActivitySourceName)
                    .SetSampler<AlwaysOnSampler>()
                    .AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.Endpoint = otlpEndpoint;
                    })
            )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter(ApplicationDiagnostics.Meter.Name)
                    .AddOtlpExporter(options =>
                        options.Endpoint = otlpEndpoint)
            )
            .WithLogging(
                logging =>
                    logging
                        .AddOtlpExporter(options =>
                            options.Endpoint = otlpEndpoint)
            );

        return services;
    }

}