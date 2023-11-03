using System.ComponentModel.DataAnnotations;

namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class LoginUserRequest
{
    [Required(ErrorMessage = "Campo de e-mail é obrigatório.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Campo de senha é obrigatório.")]
    public string Password { get; set; } = null!;
}