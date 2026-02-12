using System.ComponentModel.DataAnnotations;

namespace CardStore.DTOs;

public class CreateOrderDto
{
    [Required]
    public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    
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
}

public class CreateOrderItemDto
{
    [Required]
    public int CardId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}