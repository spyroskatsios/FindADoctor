namespace Appointments.Domain.Common;

public abstract class AggregateRoot<T> : Entity<T>, IHasDomainEvents
{
    protected AggregateRoot(T id) : base(id)
    {
    }


    protected AggregateRoot() { }

    protected readonly List<IDomainEvent> _domainEvents = [];

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();
        _domainEvents.Clear();

        return copy;
    }
}