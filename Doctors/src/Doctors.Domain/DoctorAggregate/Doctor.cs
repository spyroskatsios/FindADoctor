using Ardalis.GuardClauses;
using Doctors.Domain.Common;
using Doctors.Domain.OfficeAggregate;
using Doctors.Domain.SubscriptionAggregate;
using ErrorOr;
using Throw;

namespace Doctors.Domain.DoctorAggregate;

public class Doctor : AggregateRoot<DoctorId>
{
    public string FirstName { get; }
    public string LastName { get; }
    public Speciality Speciality { get; }
    public SubscriptionId? SubscriptionId { get; private set; }
    public string UserId { get; }

    private int _maxOffices;
    private readonly List<OfficeId> _officeIds = [];
    
    public IReadOnlyList<OfficeId> OfficeIds => _officeIds.AsReadOnly();

    public Doctor(string firstName, string lastName, 
        Speciality speciality, string userId,  DoctorId? id = null) 
        : base(id ?? DoctorId.New())
    {
        FirstName = firstName.ThrowIfNull();
        LastName = lastName.ThrowIfNull();
        Speciality = speciality;
        SubscriptionId = null;
        UserId = userId;
    }

    public ErrorOr<Success> AddOffice(Office office)
    {
        if (_officeIds.Contains(office.Id))
            return Error.Conflict(description: "Office already exists");

        if (_officeIds.Count >= _maxOffices)
            return DoctorErrors.CannotHaveMoreOfficesThanSubscriptionAllows;
        
        _officeIds.Add(office.Id);
        
        _domainEvents.Add(new OfficeCreatedEvent(this, office));

        return Result.Success;
    }
    
    public ErrorOr<Success> AddSubscription(Subscription subscription)
    {
        if (SubscriptionId is not null)
            return DoctorErrors.CannotSetSubscriptionIfAlreadyOne;

        SubscriptionId = subscription.Id;

        _maxOffices = subscription.SubscriptionType.MaxOffices;

        _domainEvents.Add(new SubscriptionSetEvent(this, subscription));
        
        return Result.Success;
    }
    
    public ErrorOr<Success> RemoveOffice(OfficeId officeId)
    {
        if (!_officeIds.Contains(officeId))
            return Error.NotFound(description: "Office not found");

        _officeIds.Remove(officeId);
        
        _domainEvents.Add(new OfficeRemovedEvent(officeId));

        return Result.Success;
    }

#pragma warning disable CS8618
    private Doctor() { }
#pragma warning restore CS8618
    
}