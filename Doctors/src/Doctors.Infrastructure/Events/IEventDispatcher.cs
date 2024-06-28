using Doctors.Domain.Common;
using FindADoctor.SharedKernel.IntegrationEvents;

namespace Doctors.Infrastructure.Events;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    Task DispatchIntegrationAsync(IntegrationEvent integrationEvent);
}