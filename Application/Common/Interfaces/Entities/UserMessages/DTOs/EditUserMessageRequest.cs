using System.ComponentModel.DataAnnotations;

namespace Application.Common.Interfaces.Entities.UserMessages.DTOs;

public class EditUserMessageRequest
{
    [Required(ErrorMessage = "Campo de id é obrigatório.")]
    public long Id { get; set; }

    [Required(ErrorMessage = "Campo de conteúdo é obrigatório.")]
    public string Content { get; set; } = null!;
}