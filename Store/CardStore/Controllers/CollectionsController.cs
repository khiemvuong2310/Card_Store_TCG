using Microsoft.AspNetCore.Mvc;
using CardStore.Services;
using CardStore.DTOs;
using CardStore.Attributes;

namespace CardStore.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CollectionsController : ControllerBase
{
    private readonly ICollectionService _collectionService;
    private readonly ILogger<CollectionsController> _logger;

    public CollectionsController(ICollectionService collectionService, ILogger<CollectionsController> logger)
    {
        _collectionService = collectionService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's collection
    /// </summary>
    /// <returns>User's card collection</returns>
    [HttpGet]
    public async Task<IActionResult> GetMyCollection()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var collection = await _collectionService.GetUserCollectionAsync(userId.Value);
            return Ok(new { success = true, data = collection, count = collection.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user collection");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get collection summary for current user
    /// </summary>
    /// <returns>Collection statistics and summary</returns>
    [HttpGet("summary")]
    public async Task<IActionResult> GetCollectionSummary()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var summary = await _collectionService.GetCollectionSummaryAsync(userId.Value);
            return Ok(new { success = true, data = summary });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection summary");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get specific card in user's collection
    /// </summary>
    /// <param name="cardId">Card ID</param>
    /// <returns>Card collection details</returns>
    [HttpGet("cards/{cardId}")]
    public async Task<IActionResult> GetCardInCollection(int cardId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var collection = await _collectionService.GetUserCardCollectionAsync(userId.Value, cardId);
            
            if (collection == null)
            {
                return NotFound(new { success = false, message = "Card not found in collection" });
            }

            return Ok(new { success = true, data = collection });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting card {CardId} in collection", cardId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Add card to collection
    /// </summary>
    /// <param name="addToCollectionDto">Card and quantity to add</param>
    /// <returns>Updated collection entry</returns>
    [HttpPost]
    public async Task<IActionResult> AddToCollection([FromBody] AddToCollectionDto addToCollectionDto)
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

            if (addToCollectionDto.Quantity <= 0)
            {
                return BadRequest(new { success = false, message = "Quantity must be greater than 0" });
            }

            var collection = await _collectionService.AddToCollectionAsync(userId.Value, addToCollectionDto);
            
            _logger.LogInformation("User {UserId} added card {CardId} (quantity: {Quantity}) to collection", 
                userId, addToCollectionDto.CardId, addToCollectionDto.Quantity);
            
            return Ok(new { success = true, data = collection });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card {CardId} to collection", addToCollectionDto.CardId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update card quantity in collection
    /// </summary>
    /// <param name="cardId">Card ID</param>
    /// <param name="updateCollectionDto">Updated collection data</param>
    /// <returns>Updated collection entry</returns>
    [HttpPut("cards/{cardId}")]
    public async Task<IActionResult> UpdateCollectionCard(int cardId, [FromBody] UpdateCollectionDto updateCollectionDto)
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

            if (updateCollectionDto.Quantity < 0)
            {
                return BadRequest(new { success = false, message = "Quantity cannot be negative" });
            }

            var collection = await _collectionService.UpdateCollectionAsync(userId.Value, cardId, updateCollectionDto);
            
            if (collection == null && updateCollectionDto.Quantity > 0)
            {
                return NotFound(new { success = false, message = "Card not found in collection" });
            }

            if (updateCollectionDto.Quantity == 0)
            {
                _logger.LogInformation("User {UserId} removed card {CardId} from collection", userId, cardId);
                return Ok(new { success = true, message = "Card removed from collection" });
            }

            _logger.LogInformation("User {UserId} updated card {CardId} quantity to {Quantity}", 
                userId, cardId, updateCollectionDto.Quantity);
            
            return Ok(new { success = true, data = collection });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card {CardId} in collection", cardId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Remove card from collection
    /// </summary>
    /// <param name="cardId">Card ID</param>
    /// <param name="quantity">Quantity to remove (optional, removes all if not specified)</param>
    /// <returns>Removal result</returns>
    [HttpDelete("cards/{cardId}")]
    public async Task<IActionResult> RemoveFromCollection(int cardId, [FromQuery] int? quantity = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            if (quantity.HasValue && quantity.Value <= 0)
            {
                return BadRequest(new { success = false, message = "Quantity must be greater than 0" });
            }

            var success = await _collectionService.RemoveFromCollectionAsync(userId.Value, cardId, quantity);
            
            if (!success)
            {
                return NotFound(new { success = false, message = "Card not found in collection" });
            }

            var message = quantity.HasValue 
                ? $"Removed {quantity.Value} card(s) from collection"
                : "Card completely removed from collection";
                
            _logger.LogInformation("User {UserId} removed card {CardId} from collection (quantity: {Quantity})", 
                userId, cardId, quantity?.ToString() ?? "all");
            
            return Ok(new { success = true, message = message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing card {CardId} from collection", cardId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get collection value for current user
    /// </summary>
    /// <returns>Total collection value</returns>
    [HttpGet("value")]
    public async Task<IActionResult> GetCollectionValue()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var value = await _collectionService.GetCollectionValueAsync(userId.Value);
            var totalCards = await _collectionService.GetTotalCardsInCollectionAsync(userId.Value);
            
            return Ok(new { 
                success = true, 
                data = new { 
                    totalValue = value,
                    totalCards = totalCards,
                    averageCardValue = totalCards > 0 ? value / totalCards : 0
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection value");
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