using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

public class EditFoundAnimalAlertRequest
{
	[Required(ErrorMessage = "Campo de id é obrigatório.")]
	public Guid Id { get; set; }

	public string? Name { get; set; }

	public string? Description { get; set; }

	[Required(ErrorMessage = "Campo de latitude é obrigatório.")]
	public double FoundLocationLatitude { get; set; }

	[Required(ErrorMessage = "Campo de longitude é obrigatório.")]
	public double FoundLocationLongitude { get; set; }

	[Required(ErrorMessage = "Campo de imagem é obrigatório.")]
	public List<IFormFile> Images { get; set; } = null!;

	[Required(ErrorMessage = "Campo de espécie é obrigatório.")]
	public int SpeciesId { get; set; }

	[Required(ErrorMessage = "Campo de idade é obrigatório.")]
	public required Age Age { get; set; }

	[Required(ErrorMessage = "Campo de porte é obrigatório.")]
	public required Size Size { get; set; }

	public Gender? Gender { get; set; }

	public int? BreedId { get; set; }

	public List<int> ColorIds { get; set; } = null!;
}