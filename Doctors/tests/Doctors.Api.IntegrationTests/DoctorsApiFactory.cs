using System.Data.Common;
using System.Net.Http.Headers;
using System.Security.Claims;
using Doctors.Api.IntegrationTests.Identity;
using Doctors.Application.Common.Authorization;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.SubscriptionAggregate;
using Doctors.Infrastructure.Persistence;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Respawn;
using Testcontainers.MsSql;
using TestsCommon.TestConstants;
using TestsCommon.Utils.Doctors;
using TestsCommon.Utils.Subscriptions;

namespace Doctors.Api.IntegrationTests;

public class DoctorsApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer =
        new MsSqlBuilder()
        .Build();
    
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    private HttpClient _httpClient = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHostedService>();
            
            services
                .RemoveAll<DbContextOptions<AppDbContext>>()
                .AddDbContext<AppDbContext>(x=>x.UseSqlServer(_dbContainer.GetConnectionString()));
            
            services.AddTestIdentity();

        });
    }
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        InitializeClient();
        CreateDatabase();
        await InitializeRespawner();
    }

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_dbConnection);
    }
    
    private async Task InitializeRespawner()
    {
        _dbConnection = Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>().Database
            .GetDbConnection();

        await _dbConnection.OpenAsync();
        
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new[] {"dbo"},
        });
    }

    private void CreateDatabase()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }

    private void InitializeClient()
    {
        _httpClient = CreateClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthenticationHandler.AuthenticationScheme);
    }
    
    
    public HttpClient GetUnAuthorizedTestClient()
    {
        _httpClient.DefaultRequestHeaders.Add(TestAuthenticationHandler.Unauthorized, "true");
        _httpClient.DefaultRequestHeaders.Remove(TestAuthenticationHandler.RoleClaimType);
        _httpClient.DefaultRequestHeaders.Remove(AppClaims.DoctorId);
        
        return _httpClient;
    }

    public HttpClient GetDoctorHttpClient(Guid? doctorId = null)
    {
        _httpClient.DefaultRequestHeaders.Remove(TestAuthenticationHandler.Unauthorized);
        _httpClient.DefaultRequestHeaders.Remove(TestAuthenticationHandler.RoleClaimType);
        _httpClient.DefaultRequestHeaders.Remove(AppClaims.DoctorId);
        
        _httpClient.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleClaimType, AppRoles.Doctor);
        _httpClient.DefaultRequestHeaders.Add(AppClaims.DoctorId, doctorId is null ? Constants.Doctor.Id.Value.ToString() : doctorId.ToString());
        
        return _httpClient;
    }

    public void CreateDoctor(Doctor? doctor = null)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Doctors.Add(doctor ?? DoctorFactory.Create());
        dbContext.SaveChanges();
    }
    
    public void AddSubscription(Subscription? subscription = null)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        subscription ??= SubscriptionFactory.Create();
        
        dbContext.Subscriptions.Add(subscription);
        
        var doctor = dbContext.Doctors.First(x => x.Id == subscription.DoctorId);
        
        doctor.AddSubscription(subscription);

        dbContext.Update(doctor);
        dbContext.SaveChanges();
    }
    
    public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await context.FindAsync<TEntity>(keyValues);
    }
    
    public async Task<List<TEntity>> FindAllAsync<TEntity>() where TEntity : class
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await context.Set<TEntity>().ToListAsync();
    }
    
    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}