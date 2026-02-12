using AutoMapper;
using CardStore.Data.Repositories;
using CardStore.DTOs;
using CardStore.Models;

namespace CardStore.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, ICardRepository cardRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _cardRepository = cardRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(id);
        return order == null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId)
    {
        var orders = await _orderRepository.GetOrdersByUserAsync(userId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
    {
        var orders = await _orderRepository.GetOrdersByStatusAsync(status);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetPendingOrdersAsync()
    {
        var orders = await _orderRepository.GetPendingOrdersAsync();
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto)
    {
        // Validate all items are in stock and calculate total
        var totalAmount = 0m;
        var orderItems = new List<OrderItem>();

        foreach (var item in createOrderDto.OrderItems)
        {
            var card = await _cardRepository.GetByIdAsync(item.CardId);
            if (card == null || !card.IsActive)
                throw new InvalidOperationException($"Card with ID {item.CardId} not found or not active");

            if (!await _cardRepository.IsCardInStockAsync(item.CardId, item.Quantity))
                throw new InvalidOperationException($"Insufficient stock for card {card.Name}");

            var orderItem = new OrderItem
            {
                CardId = item.CardId,
                Quantity = item.Quantity,
                UnitPrice = card.Price
            };

            orderItems.Add(orderItem);
            totalAmount += orderItem.Quantity * orderItem.UnitPrice;
        }

        // Create order
        var order = _mapper.Map<Order>(createOrderDto);
        order.UserId = userId;
        order.TotalAmount = totalAmount;
        order.Status = "Pending";
        order.OrderDate = DateTime.UtcNow;

        var createdOrder = await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        // Add order items
        foreach (var item in orderItems)
        {
            item.OrderId = createdOrder.Id;
        }

        // Note: You might want to create an OrderItemRepository for this
        // For now, we'll use the context directly through the order
        createdOrder.OrderItems = orderItems;
        await _orderRepository.SaveChangesAsync();

        // Update stock quantities
        foreach (var item in createOrderDto.OrderItems)
        {
            await _cardRepository.UpdateStockAsync(item.CardId, -item.Quantity);
        }
        await _cardRepository.SaveChangesAsync();

        // Return the created order with items
        var orderWithItems = await _orderRepository.GetOrderWithItemsAsync(createdOrder.Id);
        return _mapper.Map<OrderDto>(orderWithItems);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var success = await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        if (success)
            await _orderRepository.SaveChangesAsync();
        
        return success;
    }

    public async Task<bool> CancelOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
        if (order == null || order.Status != "Pending")
            return false;

        // Restore stock quantities
        foreach (var item in order.OrderItems)
        {
            await _cardRepository.UpdateStockAsync(item.CardId, item.Quantity);
        }
        await _cardRepository.SaveChangesAsync();

        // Update order status to cancelled
        return await UpdateOrderStatusAsync(orderId, "Cancelled");
    }

    public async Task<decimal> CalculateOrderTotalAsync(CreateOrderDto createOrderDto)
    {
        var total = 0m;

        foreach (var item in createOrderDto.OrderItems)
        {
            var card = await _cardRepository.GetByIdAsync(item.CardId);
            if (card != null && card.IsActive)
            {
                total += card.Price * item.Quantity;
            }
        }

        return total;
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _orderRepository.GetTotalRevenueAsync(startDate, endDate);
    }

    public async Task<int> GetTotalOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _orderRepository.GetTotalOrdersCountAsync(startDate, endDate);
    }
}