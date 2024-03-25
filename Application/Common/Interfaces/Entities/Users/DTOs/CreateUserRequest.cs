using System.ComponentModel.DataAnnotations;

namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class CreateUserRequest
{
	[Required(ErrorMessage = "Campo de nome completo é obrigatório.")]
	public string FullName { get; set; } = null!;

	[Required(ErrorMessage = "Campo de número de telefone é obrigatório.")]
	public string PhoneNumber { get; set; } = null!;

	[Required(ErrorMessage = "Campo de e-mail é obrigatório.")]
	public string Email { get; set; } = null!;

	[Required(ErrorMessage = "Campo de senha é obrigatório.")]
	public string Password { get; set; } = null!;

	[Required(ErrorMessage = "Campo de confirmar senha é obrigatório.")]
	public string ConfirmPassword { get; set; } = null!;
}