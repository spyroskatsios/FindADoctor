using System.Net;
using System.Net.Http.Json;
using Doctors.Contracts.Offices;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using TestsCommon.TestConstants;
using TestsCommon.Utils.Offices;

namespace Doctors.Api.IntegrationTests.Controllers;

[Collection(TestCollection.Name)]
public class OfficesControllerTests : IAsyncLifetime
{
    private readonly DoctorsApiFactory _doctorsApiFactory;

    public OfficesControllerTests(DoctorsApiFactory doctorsApiFactory)
    {
        _doctorsApiFactory = doctorsApiFactory;
    }
    
    [Fact]
    public async Task CreateOffice_WhenNotLoggedIn_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _doctorsApiFactory.GetUnAuthorizedTestClient();
        var request = OfficeRequestFactory.CreateCreateOfficeRequest();
        
        // Act
        var response = await client.PostAsJsonAsync($"doctors/{Constants.Doctor.Id.Value}/offices", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    
    [Fact]
    public async Task CreateOffice_WhenDoctor_ShouldCreateOffice()
    {
        // Arrange
        _doctorsApiFactory.CreateDoctor();
        _doctorsApiFactory.AddSubscription();

        var client = _doctorsApiFactory.GetDoctorHttpClient();
        var request = OfficeRequestFactory.CreateCreateOfficeRequest();
        
        // Act
        var response = await client.PostAsJsonAsync($"doctors/{Constants.Doctor.Id.Value}/offices", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var officeResponse = await response.Content.ReadFromJsonAsync<OfficeResponse>();
        officeResponse.Should().NotBeNull();

        response.Headers.Location!.PathAndQuery.Should().Be($"/offices/{officeResponse!.Id}");
        
        var office = await _doctorsApiFactory.FindAsync<Office>(OfficeId.From(officeResponse!.Id));
        office.Should().NotBeNull();
        
        var doctor = await _doctorsApiFactory.FindAsync<Doctor>(Constants.Doctor.Id);
        doctor!.OfficeIds.Should().Contain(OfficeId.From(officeResponse.Id));
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