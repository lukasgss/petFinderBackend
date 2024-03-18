using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.ValueObjects;

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

	[Required, EnumDataType(typeof(Age))]
	public required Age Age { get; set; }

	[Required, EnumDataType(typeof(Size))]
	public required Size Size { get; set; }

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
	public virtual ICollection<Vaccine> Vaccines { get; set; } = null!;
	public virtual List<PetImage> Images { get; set; } = null!;
}