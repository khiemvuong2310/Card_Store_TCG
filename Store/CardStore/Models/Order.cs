using System.ComponentModel.DataAnnotations;

namespace CardStore.Models;

public class Order
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? ShippedDate { get; set; }
    
    public DateTime? DeliveredDate { get; set; }
    
    [MaxLength(200)]
    public string? ShippingAddress { get; set; }
    
    [MaxLength(100)]
    public string? ShippingCity { get; set; }
    
    [MaxLength(20)]
    public string? ShippingPostalCode { get; set; }
    
    [MaxLength(100)]
    public string? ShippingCountry { get; set; }
    
    [MaxLength(100)]
    public string? PaymentMethod { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}