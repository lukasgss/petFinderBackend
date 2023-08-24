using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Pet
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    public Guid UserId { get; set; }

    [ForeignKey("BreedId")]
    public Breed Breed { get; set; } = null!;
    public int BreedId { get; set; }

    public virtual ICollection<Color> Colors { get; set; } = null!;
}