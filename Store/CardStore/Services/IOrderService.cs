using CardStore.DTOs;

namespace CardStore.Services;

public interface IOrderService
{
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    Task<IEnumerable<OrderDto>> GetPendingOrdersAsync();
    Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    Task<bool> CancelOrderAsync(int orderId);
    Task<decimal> CalculateOrderTotalAsync(CreateOrderDto createOrderDto);
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<int> GetTotalOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null);
}