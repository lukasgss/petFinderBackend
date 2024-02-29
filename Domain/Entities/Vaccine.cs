using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Vaccine
{
	public int Id { get; set; }

	[Required, MaxLength(50)]
	public string Name { get; set; } = null!;

	public virtual ICollection<Species> Species { get; set; } = null!;
	public virtual ICollection<Pet> Pets { get; set; } = null!;
}