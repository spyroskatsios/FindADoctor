namespace Appointments.Api.IntegrationTests;

[CollectionDefinition(Name)]
public class TestCollection  : ICollectionFixture<AppointmentsApiFactory>
{
    public const string Name = "TestBaseCollection";
}