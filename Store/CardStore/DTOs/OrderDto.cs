namespace CardStore.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public string? ShippingAddress { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? ShippingCountry { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public string CardName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}