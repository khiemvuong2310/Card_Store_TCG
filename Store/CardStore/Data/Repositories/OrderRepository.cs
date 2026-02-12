using CardStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CardStore.Data.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Card)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Card)
            .Where(o => o.Status == status)
            .OrderBy(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Card)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Card)
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .OrderBy(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
    {
        return await GetOrdersByStatusAsync("Pending");
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        return await query
            .Where(o => o.Status != "Cancelled")
            .SumAsync(o => o.TotalAmount);
    }

    public async Task<int> GetTotalOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        return await query.CountAsync();
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _dbSet.FindAsync(orderId);
        if (order == null)
            return false;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        if (status == "Shipped" && !order.ShippedDate.HasValue)
            order.ShippedDate = DateTime.UtcNow;
        else if (status == "Delivered" && !order.DeliveredDate.HasValue)
            order.DeliveredDate = DateTime.UtcNow;

        return true;
    }
}