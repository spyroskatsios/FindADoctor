using Doctors.Api;
using Doctors.Application.Common.Authorization;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Common.Models;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.SubscriptionAggregate;
using Doctors.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TestsCommon.TestConstants;
using TestsCommon.Utils.Doctors;

namespace Doctors.Application.SubcutaneousTests;

public class DoctorsApiFactory : WebApplicationFactory<IApiMarker>
{
    private ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<DbContextOptions<AppDbContext>>()
                .AddDbContext<AppDbContext>((sp, options) => options.UseInMemoryDatabase("TestDb"));

            services.RemoveAll<IHostedService>();

            services.RemoveAll<ICurrentUserService>()
                .AddSingleton(_currentUserService);

        });
    }
    
    public IMediator GetMediator()
    {
        var serviceScope = Services.CreateScope();
        ResetDatabase();
        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }
    
    public void SetCurrentUserDoctor(Guid? doctorId = null, string? userId = null)
    {
        _currentUserService.DoctorId.Returns(doctorId ?? Constants.Doctor.Id.Value);
        var currentUser = new CurrentUser(userId ?? Constants.User.Id, [], new[] {AppRoles.Doctor});
        _currentUserService.User.Returns(currentUser);
    }
    
    public AppDbContext GetDbContext()
    {
        var serviceScope = Services.CreateScope();
        return serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
    
    private void CreateDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
    
    private void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureDeleted();
        CreateDatabase();
    }

    public void CreateDoctor(Doctor doctor, Subscription? subscription = null)
    {
        var serviceScope = Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        doctor.AddSubscription(subscription ??
                               new Subscription(SubscriptionType.Premium, doctor.Id, Constants.Subscription.Id));
        dbContext.Doctors.Add(doctor);
        dbContext.SaveChanges();
    }
    
    public void CreateSubscription(Subscription subscription)
    {
        var serviceScope = Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Subscriptions.Add(subscription);
        dbContext.SaveChanges();
    }
}