using System.ComponentModel.DataAnnotations;

namespace Application.Common.Interfaces.Entities.UserMessages.DTOs;

public class SendUserMessageRequest
{
    [Required(ErrorMessage = "Campo de conteúdo é obrigatório.")]
    public string Content { get; set; } = null!;

    [Required(ErrorMessage = "Campo de recebedor é obrigatório.")]
    public Guid ReceiverId { get; set; }
}