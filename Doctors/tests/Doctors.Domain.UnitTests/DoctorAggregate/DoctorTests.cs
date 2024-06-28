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
        addOffice1Result.IsError.Should().BeFalse();
        addOffice2Result.IsError.Should().BeTrue();
        addOffice2Result.FirstError.Should().Be(DoctorErrors.CannotHaveMoreOfficesThanSubscriptionAllows);
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
        result.IsError.Should().BeFalse();
        doctor.SubscriptionId.Should().Be(subscription.Id);

        var events = doctor.PopDomainEvents();
        
        events.Should().HaveCount(1);
        events.First().Should().BeOfType<SubscriptionSetEvent>();
        
        var subscriptionSetEvent = (SubscriptionSetEvent)events.First();
        
        subscriptionSetEvent.Doctor.Should().BeEquivalentTo(doctor);
        subscriptionSetEvent.Subscription.Should().BeEquivalentTo(subscription);
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
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(DoctorErrors.CannotSetSubscriptionIfAlreadyOne);
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
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
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
        result.IsError.Should().BeFalse();

        var events = doctor.PopDomainEvents()
            .Where(x => x.GetType() == typeof(OfficeRemovedEvent)).ToList();

        events.Should().HaveCount(1);

        var officeRemovedEvent = (OfficeRemovedEvent)events.First();
        
        officeRemovedEvent.OfficeId.Should().Be(office.Id);
    }
}