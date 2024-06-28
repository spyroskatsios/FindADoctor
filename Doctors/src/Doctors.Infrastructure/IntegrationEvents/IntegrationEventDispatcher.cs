using Doctors.Infrastructure.Persistence;
using FindADoctor.SharedKernel.IntegrationEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Doctors.Infrastructure.IntegrationEvents;

public interface IIntegrationEventDispatcher
{
    Task DispatchAsync(IntegrationEvent integrationEvent);
}

public class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private readonly AppDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public IntegrationEventDispatcher( AppDbContext dbContext, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }


    public async Task DispatchAsync(IntegrationEvent integrationEvent) // Or just use MediatR with the decorator pattern
    {
        var handlers = _serviceProvider.GetServices(typeof(INotificationHandler<>).MakeGenericType(integrationEvent.GetType()));
        
        foreach (var handler in handlers)
        {
            await CheckAndDispatchAsync(integrationEvent, handler!);
        }
    }
    

    private async Task CheckAndDispatchAsync(IntegrationEvent integrationEvent, object handler)
    {
        var handlerType = handler.GetType();
        var id = integrationEvent.Id;
        var method = handlerType.GetMethod("Handle");
        
        if(await _dbContext.ConsumedIntegrationEvents.AnyAsync(x=>x.Id == id && x.Handler == handlerType.Name))
            return;
        
        var transaction = await _dbContext.BeginTransactionAsync();
        
        try
        {
            await (Task)method!.Invoke(handler, new object[] {integrationEvent, default(CancellationToken)})!;
            _dbContext.ConsumedIntegrationEvents.Add(new ConsumedIntegrationEvent(id, handlerType.Name));
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            // TODO Logging
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }
}