using Appointments.Domain.Common;
using FindADoctor.SharedKernel.IntegrationEvents;

namespace Appointments.Infrastructure.Events;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
}