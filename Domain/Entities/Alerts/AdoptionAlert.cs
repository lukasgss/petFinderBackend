using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Alerts;

public class AdoptionAlert
{
    public Guid Id { get; set; }
    
    [Required, Column(TypeName = "decimal(6, 3)")]
    public bool OnlyForScreenedProperties { get; set; }
    
    [Required, Column(TypeName = "decimal(6, 3)")] 
    public double LocationLatitude { get; set; }
    
    [Required, Column(TypeName = "decimal(6, 3)")] 
    public double LocationLongitude { get; set; }
    
    [MaxLength(500)] 
    public string? Description { get; set; }
    
    [Required]
    public DateTime RegistrationDate { get; set; }
    
    public DateOnly? AdoptionDate { get; set; }
    
    [ForeignKey("PetId")]
    public virtual Pet Pet { get; set; } = null!;
    public Guid PetId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    public Guid UserId { get; set; }
}