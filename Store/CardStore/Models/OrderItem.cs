using System.ComponentModel.DataAnnotations;

namespace CardStore.Models;

public class OrderItem
{
    public int Id { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int CardId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice => Quantity * UnitPrice;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public Card Card { get; set; } = null!;
}