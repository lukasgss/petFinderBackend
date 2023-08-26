using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Species
{
    public int Id { get; set; }
    
    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Pet> Pets { get; set; } = null!;
    public virtual ICollection<Breed> Breeds { get; set; } = null!;
}