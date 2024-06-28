namespace Appointments.Domain.Common;

public interface IHasDomainEvents
{
    List<IDomainEvent> PopDomainEvents();
}