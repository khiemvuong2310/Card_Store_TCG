using System.ComponentModel.DataAnnotations;

namespace CardStore.Models;

public class Card
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Rarity { get; set; } = string.Empty;
    
    [Required]
    public decimal Price { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Set { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Type { get; set; }
    
    public int? Attack { get; set; }
    
    public int? Defense { get; set; }
    
    public int? Level { get; set; }
    
    [MaxLength(100)]
    public string? Attribute { get; set; }
    
    public int StockQuantity { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Collection> Collections { get; set; } = new List<Collection>();
}