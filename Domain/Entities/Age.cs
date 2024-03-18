using System.ComponentModel.DataAnnotations;
using Domain.Entities.Alerts.UserPreferences;

namespace Domain.Entities;

public class Age
{
	public int Id { get; set; }

	[Required, MaxLength(15)]
	public required string Name { get; set; }

	public virtual ICollection<Pet>? Pet { get; set; }
	public virtual AdoptionUserPreferences AdoptionUserPreferences { get; set; } = null!;
	public virtual FoundAnimalUserPreferences FoundAnimalUserPreferences { get; set; } = null!;
}