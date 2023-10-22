using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.UserMessages.DTOs;

public class UserMessageResponse
{
    public long Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime TimeStampUtc { get; set; }
    public bool HasBeenRead { get; set; }
    public bool HasBeenEdited { get; set; }
    public bool HasBeenDeleted { get; set; }
    public UserDataResponse Sender { get; set; } = null!;
    public UserDataResponse Receiver { get; set; } = null!;
}