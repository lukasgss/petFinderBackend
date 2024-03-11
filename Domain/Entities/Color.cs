using System.ComponentModel.DataAnnotations;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.UserPreferences;

namespace Domain.Entities;

public class Color
{
	public int Id { get; set; }

	[Required, MaxLength(255)]
	public string Name { get; set; } = null!;

	[Required, MaxLength(255)]
	public string HexCode { get; set; } = null!;

	public virtual ICollection<Pet> Pets { get; set; } = null!;

	public virtual ICollection<FoundAnimalAlert> FoundAnimalAlerts { get; set; } = null!;
	public virtual ICollection<FoundAnimalUserPreferences> FoundAnimalUserPreferences { get; set; } = null!;
	public virtual ICollection<AdoptionUserPreferences> AdoptionUserPreferences { get; set; } = null!;
}