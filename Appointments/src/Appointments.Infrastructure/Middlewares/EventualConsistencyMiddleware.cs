using Appointments.Application.Common.Interfaces;
using Appointments.Domain.Common;
using Appointments.Infrastructure.Events;
using Appointments.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Appointments.Infrastructure.Middlewares;

public class EventualConsistencyMiddleware
{
    public const string DomainEventsKey = "DomainEventsQueue";

    private readonly RequestDelegate _next;
    private readonly ILogger<EventualConsistencyMiddleware> _logger;

    public EventualConsistencyMiddleware(RequestDelegate next, ILogger<EventualConsistencyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Kudos to Amichai for this idea of publishing domain events after the response is sent. 
    // https://www.youtube.com/@amantinband
    public async Task InvokeAsync(HttpContext context, IEventDispatcher dispatcher, AppDbContext dbContext)
    {
        var transaction = await dbContext.BeginTransactionAsync();

        context.Response.OnCompleted(async () =>
        {
            try
            {
                if (context.Items.TryGetValue(DomainEventsKey, out var value) &&
                    value is Queue<IDomainEvent> domainEventsQueue)
                {
                    while (domainEventsQueue!.TryDequeue(out var domainEvent))
                    {
                        await dispatcher.DispatchAsync(domainEvent);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Eventually Consistency Exception at {endpoint}", context.Request.Path); // TODO: Add more details
                // notify the client that even though they got a good response, the changes didn't take place
                // due to an unexpected error
                
                await transaction.RollbackAsync();
            }
            finally
            {
                await transaction.DisposeAsync();
            }

        });

        await _next(context);
    }
    
}