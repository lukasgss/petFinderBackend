using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.ValueObjects;

public class PetImage
{
	public long Id { get; set; }

	[Required, MaxLength(100)]
	public string ImageUrl { get; set; } = null!;

	[ForeignKey("PetId")]
	public virtual Pet Pet { get; set; } = null!;

	public Guid PetId { get; set; }
}