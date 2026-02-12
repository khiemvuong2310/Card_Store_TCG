namespace CardStore.DTOs;

public class CollectionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int CardId { get; set; }
    public string CardName { get; set; } = string.Empty;
    public string CardSet { get; set; } = string.Empty;
    public string CardRarity { get; set; } = string.Empty;
    public decimal CardPrice { get; set; }
    public string? CardImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime AcquiredDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CollectionSummaryDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int TotalCards { get; set; }
    public int UniqueCards { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class AddToCollectionDto
{
    public int CardId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Notes { get; set; }
}

public class UpdateCollectionDto
{
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}