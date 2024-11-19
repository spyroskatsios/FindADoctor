using Identity.Api;
using Identity.Core;
using Identity.Core.Persistence;
using Identity.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCore(builder.Configuration)
    .AddApi();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseExceptionHandler();

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

await CreateRolesAsync();

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


async Task CreateRolesAsync()
{
    await using var scope = app.Services.CreateAsyncScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var seedData = new SeedData();
    await seedData.CreateRoles(roleManager);
}


