using Domain.Entities;
using Tests.EntityGenerators;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
	public static class MissingAlertData
	{
		public static readonly Guid Id = Guid.NewGuid();
		public const string OwnerName = UserData.FullName;
		public const string OwnerPhoneNumber = UserData.PhoneNumber;
		public static readonly DateTime RegistrationDate = new DateTime(2020, 1, 1);
		public const double LastSeenLocationLatitude = 90;
		public const double LastSeenLocationLongitude = 90;
		public const string Description = "Description";
		public static readonly DateOnly? NonRecoveredRecoveryDate = null;
		public static readonly DateOnly RecoveryDate = DateOnly.FromDateTime(new DateTime(2020, 1, 1));
		public static readonly Pet Pet = PetGenerator.GeneratePet();
		public static readonly Guid PetId = Pet.Id;
		public static readonly User User = UserGenerator.GenerateUser();
		public static readonly Guid UserId = User.Id;
	}
}