using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities;

namespace Domain.Entities.Alerts;

public class MissingAlert
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string OwnerName { get; set; } = null!;
    
    [Required, MaxLength(30)]
    public string OwnerPhoneNumber { get; set; } = null!;
    
    [Required]
    public DateTime RegistrationDate { get; set; }

    [Required, Column(TypeName = "decimal(6, 3)")]
    public double LastSeenLocationLatitude { get; set; }

    [Required, Column(TypeName = "decimal(6, 3)")]
    public double LastSeenLocationLongitude { get; set; }

    public bool PetHasBeenRecovered { get; set; } = false;

    [Required, ForeignKey("PetId")] 
    public virtual Pet Pet { get; set; } = null!;
    public Guid PetId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    public Guid? UserId { get; set; }
}