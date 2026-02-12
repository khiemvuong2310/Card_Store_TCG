namespace CardStore.DTOs;

public class CardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string Set { get; set; } = string.Empty;
    public string? Type { get; set; }
    public int? Attack { get; set; }
    public int? Defense { get; set; }
    public int? Level { get; set; }
    public string? Attribute { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}