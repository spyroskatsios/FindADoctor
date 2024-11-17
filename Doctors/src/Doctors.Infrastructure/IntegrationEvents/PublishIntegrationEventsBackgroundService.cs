using System.Diagnostics;
using System.Text.Json;
using Doctors.Infrastructure.Persistence;
using FindADoctor.SharedKernel.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Throw;

namespace Doctors.Infrastructure.IntegrationEvents;

public class PublishIntegrationEventsBackgroundService : IHostedService
{
    private Task? _doWorkTask = null;
    private PeriodicTimer? _timer = null!;
    private readonly IIntegrationEventsPublisher _integrationEventPublisher;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<PublishIntegrationEventsBackgroundService> _logger;
    private readonly CancellationTokenSource _cts;

    public PublishIntegrationEventsBackgroundService(IIntegrationEventsPublisher integrationEventPublisher, IServiceScopeFactory serviceScopeFactory,
        ILogger<PublishIntegrationEventsBackgroundService> logger)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _cts = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _doWorkTask = DoWorkAsync();

        return Task.CompletedTask;
    }
    
    private async Task DoWorkAsync()
    {
        _logger.LogInformation("Starting integration event publisher background service.");

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            try
            {
                await PublishIntegrationEventsFromDbAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while publishing integration events.");
            }
        }
    }
    
    private async Task PublishIntegrationEventsFromDbAsync()
    {
        await Task.Delay(6000);
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var outboxIntegrationEvents = dbContext.OutboxIntegrationEvents.ToList();

        _logger.LogInformation("Read a total of {NumEvents} outbox integration events", outboxIntegrationEvents.Count);

        if(outboxIntegrationEvents.Count == 0)
            return;
        
        outboxIntegrationEvents.ForEach(outboxIntegrationEvent =>
        {
            var integrationEvent = JsonSerializer.Deserialize<IntegrationEvent>(outboxIntegrationEvent.EventContent);
            integrationEvent.ThrowIfNull();

            
            Dictionary<string, string>? extractedContext = null;
            
            if (outboxIntegrationEvent.ActivityExtractedContext is not null)
                extractedContext = JsonSerializer.Deserialize<Dictionary<string, string>>(outboxIntegrationEvent.ActivityExtractedContext);

            _logger.LogInformation("Publishing event of type: {EventType}", integrationEvent.GetType().Name);
            _integrationEventPublisher.PublishEvent(integrationEvent, extractedContext);
            _logger.LogInformation("Integration event published successfully");
        });

        dbContext.RemoveRange(outboxIntegrationEvents);
        await dbContext.SaveChangesAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_doWorkTask is null)
            return;

        await _cts.CancelAsync();
        await _doWorkTask;

        _timer?.Dispose();
        _cts.Dispose();
    }
}