using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.UserMessages.DTOs;

public class UserMessageResponse
{
	public long Id { get; init; }
	public string Content { get; init; } = null!;
	public DateTime TimeStampUtc { get; init; }
	public bool HasBeenRead { get; init; }
	public bool HasBeenEdited { get; init; }
	public bool HasBeenDeleted { get; init; }
	public UserDataResponse Sender { get; init; } = null!;
	public UserDataResponse Receiver { get; init; } = null!;
}