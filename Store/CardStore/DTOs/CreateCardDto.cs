using System.ComponentModel.DataAnnotations;

namespace CardStore.DTOs;

public class CreateCardDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Rarity { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [MaxLength(500)]
    [Url]
    public string? ImageUrl { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Set { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Type { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? Attack { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? Defense { get; set; }
    
    [Range(1, 12)]
    public int? Level { get; set; }
    
    [MaxLength(100)]
    public string? Attribute { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    public bool IsActive { get; set; } = true;
}