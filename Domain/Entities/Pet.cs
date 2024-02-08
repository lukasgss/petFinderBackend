using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class Pet
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Observations { get; set; }

    [Required, EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
    
    public int? AgeInMonths { get; set; }

    // TODO: Add the functionality of adding multiple images
    [MaxLength(100)]
    public string Image { get; set; } = null!;

    [Required, ForeignKey("UserId")] 
    public virtual User Owner { get; set; } = null!;
    public Guid UserId { get; set; }

    [ForeignKey("BreedId")]
    public Breed Breed { get; set; } = null!;
    public int BreedId { get; set; }

    [ForeignKey("SpeciesId")]
    public virtual Species Species { get; set; } = null!;
    public int SpeciesId { get; set; }

    public virtual ICollection<Color> Colors { get; set; } = null!;
}