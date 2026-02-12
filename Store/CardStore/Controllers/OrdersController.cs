using Microsoft.AspNetCore.Mvc;
using CardStore.Services;
using CardStore.DTOs;
using CardStore.Attributes;

namespace CardStore.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's orders
    /// </summary>
    /// <returns>List of orders for the current user</returns>
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var orders = await _orderService.GetOrdersByUserAsync(userId.Value);
            return Ok(new { success = true, data = orders, count = orders.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user orders");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get a specific order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new { success = false, message = "Order not found" });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            // Check if user owns this order or is admin
            if (order.UserId != userId.Value) // In real app, also check if user is admin
            {
                return Forbid("Access denied");
            }

            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    /// <param name="createOrderDto">Order creation data</param>
    /// <returns>Created order</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            if (!createOrderDto.OrderItems.Any())
            {
                return BadRequest(new { success = false, message = "Order must contain at least one item" });
            }

            var order = await _orderService.CreateOrderAsync(userId.Value, createOrderDto);
            
            _logger.LogInformation("Order {OrderId} created successfully for user {UserId}", order.Id, userId);
            
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, 
                new { success = true, data = order });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Calculate order total before placing order
    /// </summary>
    /// <param name="createOrderDto">Order data to calculate</param>
    /// <returns>Calculated total amount</returns>
    [HttpPost("calculate-total")]
    public async Task<IActionResult> CalculateOrderTotal([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
            }

            if (!createOrderDto.OrderItems.Any())
            {
                return BadRequest(new { success = false, message = "Order must contain at least one item" });
            }

            var total = await _orderService.CalculateOrderTotalAsync(createOrderDto);
            return Ok(new { success = true, totalAmount = total, itemCount = createOrderDto.OrderItems.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating order total");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Cancel an order (only if status is Pending)
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Cancellation result</returns>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new { success = false, message = "Order not found" });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            // Check if user owns this order
            if (order.UserId != userId.Value)
            {
                return Forbid("Access denied");
            }

            var success = await _orderService.CancelOrderAsync(id);
            
            if (!success)
            {
                return BadRequest(new { success = false, message = "Order cannot be cancelled" });
            }

            _logger.LogInformation("Order {OrderId} cancelled by user {UserId}", id, userId);
            return Ok(new { success = true, message = "Order cancelled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get orders by status (Admin only)
    /// </summary>
    /// <param name="status">Order status</param>
    /// <returns>Orders with the specified status</returns>
    [HttpGet("by-status/{status}")]
    // Note: In a real application, you'd check for admin role here
    public async Task<IActionResult> GetOrdersByStatus(string status)
    {
        try
        {
            // Note: In a real application, you'd verify admin role here
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(new { success = true, data = orders, count = orders.Count(), status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by status {Status}", status);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get pending orders (Admin only)
    /// </summary>
    /// <returns>All pending orders</returns>
    [HttpGet("pending")]
    // Note: In a real application, you'd check for admin role here
    public async Task<IActionResult> GetPendingOrders()
    {
        try
        {
            // Note: In a real application, you'd verify admin role here
            var orders = await _orderService.GetPendingOrdersAsync();
            return Ok(new { success = true, data = orders, count = orders.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending orders");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update order status (Admin only)
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="status">New status</param>
    /// <returns>Update result</returns>
    [HttpPut("{id}/status")]
    // Note: In a real application, you'd check for admin role here
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new { success = false, message = "Status is required" });
            }

            // Note: In a real application, you'd verify admin role here
            var success = await _orderService.UpdateOrderStatusAsync(id, status);
            
            if (!success)
            {
                return NotFound(new { success = false, message = "Order not found" });
            }

            _logger.LogInformation("Order {OrderId} status updated to {Status}", id, status);
            return Ok(new { success = true, message = "Order status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId} status", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get revenue statistics (Admin only)
    /// </summary>
    /// <param name="startDate">Start date for revenue calculation</param>
    /// <param name="endDate">End date for revenue calculation</param>
    /// <returns>Revenue and order count statistics</returns>
    [HttpGet("stats/revenue")]
    // Note: In a real application, you'd check for admin role here
    public async Task<IActionResult> GetRevenueStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            // Note: In a real application, you'd verify admin role here
            var totalRevenue = await _orderService.GetTotalRevenueAsync(startDate, endDate);
            var totalOrders = await _orderService.GetTotalOrdersCountAsync(startDate, endDate);
            
            return Ok(new { 
                success = true, 
                data = new { 
                    totalRevenue = totalRevenue,
                    totalOrders = totalOrders,
                    averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0,
                    startDate = startDate,
                    endDate = endDate
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue statistics");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = HttpContext.Items["UserId"];
        if (userIdClaim != null && int.TryParse(userIdClaim.ToString(), out int userId))
        {
            return userId;
        }
        return null;
    }
}