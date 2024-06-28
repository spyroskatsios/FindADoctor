using System.Net;
using System.Net.Http.Json;
using Doctors.Contracts.Subscriptions;
using Doctors.Domain.SubscriptionAggregate;
using TestsCommon.Utils.Subscriptions;

namespace Doctors.Api.IntegrationTests.Controllers;

[Collection(TestCollection.Name)]
public class SubscriptionsControllerTests : IAsyncLifetime
{
    private readonly DoctorsApiFactory _doctorsApiFactory;
    
    [Fact]
    public async Task CreateSubscription__ShouldCreateSubscription()
    {
        // Arrange
        _doctorsApiFactory.CreateDoctor();

        var client = _doctorsApiFactory.GetDoctorHttpClient();
        var request = SubscriptionRequestFactory.CreateCreateSubscriptionRequest();
        
        // Act
        var response = await client.PostAsJsonAsync($"subscriptions", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var subscriptionResponse = await response.Content.ReadFromJsonAsync<SubscriptionResponse>();
        subscriptionResponse.Should().NotBeNull();
        subscriptionResponse!.SubscriptionType.Should().Be(request.SubscriptionType);

        response.Headers.Location!.PathAndQuery.Should().Be($"/subscriptions/{subscriptionResponse.Id}");
        
        var subscription = await _doctorsApiFactory.FindAsync<Subscription>(SubscriptionId.From(subscriptionResponse.Id));
        subscription.Should().NotBeNull();
    }

    public SubscriptionsControllerTests(DoctorsApiFactory doctorsApiFactory)
    {
        _doctorsApiFactory = doctorsApiFactory;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _doctorsApiFactory.ResetDatabase();
    }
}