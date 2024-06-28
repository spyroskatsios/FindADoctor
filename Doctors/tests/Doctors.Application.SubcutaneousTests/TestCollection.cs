namespace Doctors.Application.SubcutaneousTests;

[CollectionDefinition(Name)]
public class TestCollection  : ICollectionFixture<DoctorsApiFactory>
{
    public const string Name = "TestCollection";
}