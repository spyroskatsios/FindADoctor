namespace Doctors.Domain.Common;

public interface IHasDomainEvents
{
    List<IDomainEvent> PopDomainEvents();
}