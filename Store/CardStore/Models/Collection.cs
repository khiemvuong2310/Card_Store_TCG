using System.ComponentModel.DataAnnotations;

namespace CardStore.Models;

public class Collection
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int CardId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    public DateTime AcquiredDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Card Card { get; set; } = null!;
}