using Domain.Entities;
using Domain.Entities.Alerts;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
	public static class AdoptionAlertCommentData
	{
		public static readonly Guid Id = Guid.NewGuid();
		public const string Content = "Content";
		public static readonly DateTime Date = new(2020, 1, 1);
		public static readonly User User = UserGenerator.GenerateUser();
		public static readonly Guid UserId = User.Id;
		public static readonly AdoptionAlert AdoptionAlert = AdoptionAlertGenerator.GenerateAdoptedAdoptionAlert();
		public static readonly Guid AdoptionAlertId = AdoptionAlert.Id;
	}
}