using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Age
{
	public int Id { get; set; }

	[Required, MaxLength(15)]
	public required string Name { get; set; }

	public virtual ICollection<Pet>? Pet { get; set; }
}