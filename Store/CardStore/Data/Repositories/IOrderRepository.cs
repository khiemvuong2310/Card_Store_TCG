using CardStore.Models;

namespace CardStore.Data.Repositories;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
    Task<Order?> GetOrderWithItemsAsync(int orderId);
    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Order>> GetPendingOrdersAsync();
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<int> GetTotalOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
}