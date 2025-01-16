using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using Doctors.Domain.SubscriptionAggregate;
using ErrorOr;
using TestsCommon.Utils.Doctors;
using TestsCommon.Utils.Offices;
using TestsCommon.Utils.Subscriptions;

namespace Doctors.Domain.UnitTests.DoctorAggregate;

public class DoctorTests
{
    [Fact]
    public void AddOffice_WhenMoreThanSubscriptionAllows_ShouldFail()
    {
        // Arrange
        var doctor = DoctorFactory.Create();
        doctor.AddSubscription(SubscriptionFactory.Create(SubscriptionType.Basic));
        var office1 = OfficeFactory.Create(id: OfficeId.New());
        var office2 = OfficeFactory.Create(id: OfficeId.New());
        
        // Act
        var addOffice1Result = doctor.AddOffice(office1);
        var addOffice2Result = doctor.AddOffice(office2);

        // Assert
        addOffice1Result.IsError.ShouldBeFalse();
        addOffice2Result.IsError.ShouldBeTrue();
        addOffice2Result.FirstError.ShouldBe(DoctorErrors.CannotHaveMoreOfficesThanSubscriptionAllows);
    }
    
    [Fact]
    public void AddSubscription_ShouldAddSubscriptionAndRaiseEvent()
    {
        // Arrange
        var doctor = DoctorFactory.Create();
        var subscription = SubscriptionFactory.Create();
        
        // Act
        var result = doctor.AddSubscription(subscription);
        
        // Assert
        result.IsError.ShouldBeFalse();
        doctor.SubscriptionId.ShouldBe(subscription.Id);

        var events = doctor.PopDomainEvents().ToList();
        
        events.Count.ShouldBe(1);
        events.First().ShouldBeOfType<SubscriptionSetEvent>();
        
        var subscriptionSetEvent = (SubscriptionSetEvent)events.First();
        
        subscriptionSetEvent.Doctor.ShouldBeEquivalentTo(doctor);
        subscriptionSetEvent.Subscription.ShouldBeEquivalentTo(subscription);
    }
    
    [Fact]
    public void AddSubscription_WhenAlreadyHasOne_ShouldFail()
    {
        // Arrange
        var doctor = DoctorFactory.Create();
        doctor.AddSubscription(SubscriptionFactory.Create());
        var subscription = SubscriptionFactory.Create();
        
        // Act
        var result = doctor.AddSubscription(subscription);
        
        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(DoctorErrors.CannotSetSubscriptionIfAlreadyOne);
    }
    
    [Fact]
    public void RemoveOffice_WhenOfficeDoesNotExist_ShouldFail()
    {
        // Arrange
        var doctor = DoctorFactory.Create();
        var office = OfficeFactory.Create();
        
        // Act
        var result = doctor.RemoveOffice(office.Id);
        
        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.NotFound);
    }
    
    [Fact]
    public void RemoveOffice_WhenOfficeExists_ShouldRemoveOfficeAndRaiseEvent()
    {
        // Arrange
        var doctor = DoctorFactory.Create();
        doctor.AddSubscription(SubscriptionFactory.Create(SubscriptionType.Basic));
        var office = OfficeFactory.Create();
        doctor.AddOffice(office);
        
        // Act
        var result = doctor.RemoveOffice(office.Id);
        
        // Assert
        result.IsError.ShouldBeFalse();

        var events = doctor.PopDomainEvents()
            .Where(x => x.GetType() == typeof(OfficeRemovedEvent)).ToList();

        events.Count.ShouldBe(1);

        var officeRemovedEvent = (OfficeRemovedEvent)events.First();
        
        officeRemovedEvent.OfficeId.ShouldBe(office.Id);
    }
}