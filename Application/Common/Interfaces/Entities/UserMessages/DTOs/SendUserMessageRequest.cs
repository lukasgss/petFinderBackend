namespace Application.Common.Interfaces.Entities.UserMessages.DTOs;

public class SendUserMessageRequest
{
    public string Content { get; set; } = null!;
    public Guid ReceiverId { get; set; }
}