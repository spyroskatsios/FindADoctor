using Doctors.Domain.Common;
using Doctors.Domain.OfficeAggregate;
using Doctors.Domain.SubscriptionAggregate;

namespace Doctors.Domain.DoctorAggregate;

public record OfficeCreatedEvent(Doctor Doctor, Office Office) : IDomainEvent;

public record SubscriptionSetEvent(Doctor Doctor, Subscription Subscription) : IDomainEvent;

public record OfficeRemovedEvent(OfficeId OfficeId) : IDomainEvent;