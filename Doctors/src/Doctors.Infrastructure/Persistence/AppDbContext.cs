using System.Reflection;
using Doctors.Application.Common.Interfaces;
using Doctors.Domain.Common;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using Doctors.Domain.SubscriptionAggregate;
using Doctors.Infrastructure.Events;
using Doctors.Infrastructure.IntegrationEvents;
using Doctors.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Doctors.Infrastructure.Persistence;

public class AppDbContext : DbContext, IReadDbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Office> Offices { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents { get; set; }
    public DbSet<ConsumedIntegrationEvent> ConsumedIntegrationEvents { get; set; }
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventDispatcher _eventDispatcher;
    
    public AppDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor, IEventDispatcher eventDispatcher) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _eventDispatcher = eventDispatcher;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(x => x)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        var domainEventsQueue = _httpContextAccessor.HttpContext is not null &&
                                _httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value) &&
                                value is Queue<IDomainEvent> existingDomainEvents
            ? existingDomainEvents
            : new Queue<IDomainEvent>();

        domainEvents.ForEach(domainEventsQueue.Enqueue);
        
        if (_httpContextAccessor.HttpContext != null)
        {
            _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
            return result;
        }
           
        await DispatchEventsAsync(domainEventsQueue);
        return result;
    }

    private async Task DispatchEventsAsync(Queue<IDomainEvent> domainEventsQueue)
    {
        while (domainEventsQueue!.TryDequeue(out var domainEvent))
        {
            await _eventDispatcher.DispatchAsync(domainEvent);
        }
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
       return await Database.BeginTransactionAsync();
    }
    
}