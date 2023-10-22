namespace Application.Common.Interfaces.Entities.UserMessages.DTOs;

public class EditUserMessageRequest
{
    public long Id { get; set; }
    public string Content { get; set; } = null!;
}