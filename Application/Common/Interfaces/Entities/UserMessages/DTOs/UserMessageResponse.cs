using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.UserMessages.DTOs;

public class UserMessageResponse
{
    public long Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public bool HasBeenRead { get; set; }
    public UserDataResponse Sender { get; set; } = null!;
    public UserDataResponse Receiver { get; set; } = null!;
}