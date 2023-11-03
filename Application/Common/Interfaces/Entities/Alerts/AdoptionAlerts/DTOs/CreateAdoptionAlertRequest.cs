using System.ComponentModel.DataAnnotations;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public class CreateAdoptionAlertRequest
{
    [Required(ErrorMessage = "Campo de apenas imóveis telados é obrigatório.")]
    public bool OnlyForScreenedProperties { get; set; }

    [Required(ErrorMessage = "Campo de latitude é obrigatório.")]
    public double LocationLatitude { get; set; }

    [Required(ErrorMessage = "Campo de longitude é obrigatório.")]
    public double LocationLongitude { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Campo do pet é obrigatório.")]
    public Guid PetId { get; set; }
}