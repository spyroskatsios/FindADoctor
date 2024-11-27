using System.Reflection;
using Appointments.Api.Diagnostics;
using Appointments.Infrastructure.Settings;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Appointments.Api.Installers;

public static class OpenTelemetryInstaller
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var otlpEndpoint = new Uri(configuration.GetValue<string>("OTLP_Endpoint")!);
        
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddService("Appointments",
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