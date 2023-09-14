using Domain.Entities;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
    public static class AdoptionAlertData
    {
        public static readonly Guid Id = Guid.NewGuid();
        public const bool OnlyForScreenedProperties = false;
        public const double LocationLatitude = 45;
        public const double LocationLongitude = 60;
        public const string Description = "Description";
        public static readonly DateTime RegistrationDate = new DateTime(2020, 1, 1);
        public static readonly DateOnly AdoptedAdoptionDate = DateOnly.FromDateTime(RegistrationDate);
        public static readonly DateOnly? NonAdoptedAdoptionDate = null;
        public static readonly Pet Pet = PetGenerator.GeneratePet();
        public static readonly Guid PetId = Pet.Id;
        public static readonly User User = UserGenerator.GenerateUser();
        public static readonly Guid UserId = User.Id;
    }
}