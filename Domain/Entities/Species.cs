using System.ComponentModel.DataAnnotations;
using Domain.Entities.Alerts.UserPreferences;

namespace Domain.Entities;

public class Species
{
	public int Id { get; set; }

	[Required, MaxLength(255)]
	public string Name { get; set; } = null!;

	public virtual ICollection<Pet> Pets { get; set; } = null!;
	public virtual ICollection<Breed> Breeds { get; set; } = null!;
	public virtual ICollection<Vaccine> Vaccines { get; set; } = null!;
	public virtual ICollection<FoundAnimalUserPreferences>? FoundAnimalUserPreferences { get; set; }
	public virtual ICollection<AdoptionUserPreferences>? AdoptionUserPreferences { get; set; }
}