namespace TestsCommon.TestConstants;

public static partial class Constants
{
    public static class User
    {
        public static readonly string Id = Guid.NewGuid().ToString();
        public const string UserName = "spyros";
        public const string Email = "spyros@sp.com";
        public const string Password = "MyPass1!";
    }

}