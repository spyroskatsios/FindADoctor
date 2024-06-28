namespace Appointments.Application.SubcutaneousTests;

[CollectionDefinition(Name)]
public class TestCollection  : ICollectionFixture<AppointmentsFactory>
{
    public const string Name = "TestCollection";
}