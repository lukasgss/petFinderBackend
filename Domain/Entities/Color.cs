using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Color
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(255)]
    public string HexCode { get; set; } = null!;
    
    public virtual ICollection<Pet> Pets { get; set; } = null!;
}