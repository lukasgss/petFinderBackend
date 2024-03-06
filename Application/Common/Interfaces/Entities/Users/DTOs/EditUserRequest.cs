using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class EditUserRequest
{
	[Required(ErrorMessage = "Campo de id é obrigatório.")]
	public required Guid Id { get; set; }

	[Required(ErrorMessage = "Campo de nome completo é obrigatório.")]
	public required string FullName { get; set; } = null!;

	[Required(ErrorMessage = "Campo de número de telefone é obrigatório.")]
	public required string PhoneNumber { get; set; } = null!;

	public IFormFile? Image { get; set; }
}