using System.ComponentModel.DataAnnotations;

namespace Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;

public class EditMissingAlertRequest
{
	[Required(ErrorMessage = "Campo de id é obrigatório.")]
	public Guid Id { get; set; }

	[Required(ErrorMessage = "Campo de latitude é obrigatório.")]
	public double LastSeenLocationLatitude { get; set; }

	[Required(ErrorMessage = "Campo de longitude é obrigatório.")]
	public double LastSeenLocationLongitude { get; set; }

	public string? Description { get; set; }

	[Required(ErrorMessage = "Campo do pet é obrigatório.")]
	public Guid PetId { get; set; }
}