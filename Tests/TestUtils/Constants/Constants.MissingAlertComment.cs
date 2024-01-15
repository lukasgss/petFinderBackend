using Domain.Entities;
using Domain.Entities.Alerts;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
	public static class MissingAlertCommentData
	{
		public static readonly Guid Id = Guid.NewGuid();
		public const string Content = "Content";
		public static readonly User User = UserGenerator.GenerateUser();
		public static readonly Guid UserId = User.Id;
		public static readonly DateTime Date = new(2020, 1, 1);
		public static readonly MissingAlert MissingAlert = MissingAlertGenerator.GenerateMissingAlert();
		public static readonly Guid MissingAlertId = MissingAlert.Id;
	}
}