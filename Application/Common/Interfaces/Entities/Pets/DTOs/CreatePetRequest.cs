using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public class CreatePetRequest
{
	[Required(ErrorMessage = "Campo de nome é obrigatório.")]
	public string Name { get; set; } = null!;

	public string? Observations { get; set; }

	[Required(ErrorMessage = "Campo de gênero é obrigatório.")]
	public Gender Gender { get; set; }

	public int? AgeId { get; set; }

	[Required(ErrorMessage = "Campo de imagens é obrigatório.")]
	public List<IFormFile> Images { get; set; } = null!;

	[Required(ErrorMessage = "Campo de raça é obrigatório.")]
	public int BreedId { get; set; }

	[Required(ErrorMessage = "Campo de espécie é obrigatório.")]
	public int SpeciesId { get; set; }

	[Required(ErrorMessage = "Campo de cores é obrigatório.")]
	public List<int> ColorIds { get; set; } = null!;

	public List<int>? VaccineIds { get; set; }
}