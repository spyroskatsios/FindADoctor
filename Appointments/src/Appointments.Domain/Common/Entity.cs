namespace Appointments.Domain.Common;

public abstract class Entity<T> 
{
    public T Id { get;  }

    protected Entity(T id) => Id = id;
    
    public override bool Equals(object? obj)
    {
        return obj is Entity<T> entity && Id!.Equals(entity.Id);
    }
    
    public static bool operator ==(Entity<T> left, Entity<T> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<T> left, Entity<T> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Id!.GetHashCode();
    }
    
#pragma warning disable CS8618
    protected Entity() { }
#pragma warning restore CS8618
}