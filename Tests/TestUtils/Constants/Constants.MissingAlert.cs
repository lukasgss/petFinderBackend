using Domain.Entities;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
    public static class MissingAlertData
    {
        public static readonly Guid Id = Guid.NewGuid();
        public const string OwnerName = Constants.UserData.FullName;
        public const string OwnerPhoneNumber = Constants.UserData.PhoneNumber;
        public static readonly DateTime RegistrationDate = new DateTime(2020, 1, 1);
        public const double LastSeenLocationLatitude = 90;
        public const double LastSeenLocationLongitude = 90;
        public const bool PetHasBeenRecovered = Constants.PetData.PetHasBeenRecovered;
        public static readonly Pet Pet = PetGenerator.GeneratePet();
        public static readonly Guid PetId = Pet.Id;
        public static readonly User? User = UserGenerator.GenerateUser();
        public static readonly Guid UserId = User.Id;
    }
}