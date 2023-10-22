using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class UserMessage
{
    public long Id { get; set; }
    
    [Required, MaxLength(500)]
    public string Content { get; set; } = null!;
    
    [Required]
    public DateTime TimeStampUtc { get; set; }
    
    [Required]
    public bool HasBeenRead { get; set; }
    
    [Required]
    public bool HasBeenEdited { get; set; }
    
    [Required]
    public bool HasBeenDeleted { get; set; }
    
    [Required, ForeignKey("SenderId")] 
    public virtual User Sender { get; set; } = null!;
    public Guid SenderId { get; set; }
    
    [Required, ForeignKey("ReceiverId")] 
    public virtual User Receiver { get; set; } = null!;
    public Guid ReceiverId { get; set; }
}