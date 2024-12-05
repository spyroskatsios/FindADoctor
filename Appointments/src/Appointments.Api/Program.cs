using System.Text.Json;
using Appointments.Api;
using Appointments.Application;
using Appointments.Contracts.Common;
using Appointments.Infrastructure;
using Appointments.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

// builder.Host.UseSerilog((context, configuration) =>
//     configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseExceptionHandler();
app.AddInfrastructureMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await MigrateAsync(app);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new HealthCheckResponse(report.Status.ToString(), report.Entries.Select(x => new HealthCheck(x.Value.Status.ToString(),
            x.Key, x.Value.Description ?? string.Empty)), report.TotalDuration);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});


app.Run();
return;

async Task MigrateAsync(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    
    if(dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory") // For SubcutaneousTests
        return;
    
    await dbContext.Database.MigrateAsync();
}

// Microsoft.EntityFrameworkCore.InMemory