using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

public class CreateFoundAnimalAlertRequest
{
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Campo de latitude é obrigatório.")]
    public double FoundLocationLatitude { get; set; }
    
    [Required(ErrorMessage = "Campo de longitude é obrigatório.")]
    public double FoundLocationLongitude { get; set; }
    
    [Required(ErrorMessage = "Campo de imagem é obrigatório.")]
    public IFormFile Image { get; set; } = null!;
    
    [Required(ErrorMessage = "Campo de espécie é obrigatório.")]
    public int SpeciesId { get; set; }
    
    public int? BreedId { get; set; }
}