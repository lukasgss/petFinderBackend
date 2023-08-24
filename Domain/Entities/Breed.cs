using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Breed
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;
}