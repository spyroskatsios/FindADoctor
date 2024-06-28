namespace Doctors.Api.IntegrationTests;

[CollectionDefinition(Name)]
public class  TestCollection  : ICollectionFixture<DoctorsApiFactory>
{
    public const string Name = "TestBaseCollection";
}