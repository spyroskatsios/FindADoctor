using Appointments.Api;
using Appointments.Application.Common.Authorization;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Common.Models;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate;
using Appointments.Domain.OfficeAggregate.Entities;
using Appointments.Domain.PatientAggregate;
using Appointments.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TestCommon.TestConstants;
using TestCommon.Utils.Common;

namespace Appointments.Application.SubcutaneousTests;

public class AppointmentsFactory : WebApplicationFactory<IApiMarker>
{
    private ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>()
                .AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

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
    
    public void SetCurrentUsePatient(Guid? patientId = null, string? userId = null)
    {
        _currentUserService.PatientId.Returns(patientId ?? Constants.Patient.Id.Value);
        var currentUser = new CurrentUser(userId ?? Constants.User.Id, [], new[] {AppRoles.Patient});
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

    public void CreateOffice(Office office)
    {
        var serviceScope = Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Offices.Add(office);
        dbContext.SaveChanges();
    }
    
    public void CreatePatient(Patient patient)
    {
        var serviceScope = Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Patients.Add(patient);
        dbContext.SaveChanges();
    }
}