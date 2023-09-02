namespace Tests.TestUtils.Constants;

public static partial class Constants
{
    public static class UserData
    {
        public static readonly Guid Id = Guid.NewGuid();
        public const string FullName = "Full Name";
        public const string PhoneNumber = "(11) 11111-1111";
        public const string UserName = "email@email.com";
        public const string Email = "email@email.com";
        public const bool EmailConfirmed = true;
        public const string Password = "Password";
        public const string JwtToken = "JwtToken";
    }
}