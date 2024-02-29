using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Breed
{
	public int Id { get; set; }

	[Required, MaxLength(255)]
	public string Name { get; set; } = null!;

	[ForeignKey("SpeciesId")]
	public virtual Species Species { get; set; } = null!;

	public int SpeciesId { get; set; }

	public virtual ICollection<Pet> Pets { get; set; } = null!;
}