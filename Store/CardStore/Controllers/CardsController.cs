using Microsoft.AspNetCore.Mvc;
using CardStore.Services;
using CardStore.DTOs;
using CardStore.Attributes;

namespace CardStore.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;
    private readonly ILogger<CardsController> _logger;

    public CardsController(ICardService cardService, ILogger<CardsController> logger)
    {
        _cardService = cardService;
        _logger = logger;
    }

    /// <summary>
    /// Get all active cards
    /// </summary>
    /// <returns>List of all active cards</returns>
    [HttpGet]
    public async Task<IActionResult> GetCards([FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        try
        {
            IEnumerable<CardDto> cards;

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                cards = await _cardService.GetPagedCardsAsync(pageNumber.Value, pageSize.Value);
            }
            else
            {
                cards = await _cardService.GetActiveCardsAsync();
            }

            return Ok(new { success = true, data = cards, count = cards.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get a specific card by ID
    /// </summary>
    /// <param name="id">Card ID</param>
    /// <returns>Card details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCard(int id)
    {
        try
        {
            var card = await _cardService.GetCardByIdAsync(id);
            
            if (card == null)
            {
                return NotFound(new { success = false, message = "Card not found" });
            }

            return Ok(new { success = true, data = card });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting card {CardId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Search cards by name, description, set, or type
    /// </summary>
    /// <param name="q">Search query</param>
    /// <returns>Matching cards</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchCards([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new { success = false, message = "Search query is required" });
            }

            var cards = await _cardService.SearchCardsAsync(q);
            return Ok(new { success = true, data = cards, count = cards.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching cards with query {Query}", q);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get cards by set
    /// </summary>
    /// <param name="set">Card set name</param>
    /// <returns>Cards from the specified set</returns>
    [HttpGet("by-set/{set}")]
    public async Task<IActionResult> GetCardsBySet(string set)
    {
        try
        {
            var cards = await _cardService.GetCardsBySetAsync(set);
            return Ok(new { success = true, data = cards, count = cards.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards by set {Set}", set);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get cards by rarity
    /// </summary>
    /// <param name="rarity">Card rarity</param>
    /// <returns>Cards with the specified rarity</returns>
    [HttpGet("by-rarity/{rarity}")]
    public async Task<IActionResult> GetCardsByRarity(string rarity)
    {
        try
        {
            var cards = await _cardService.GetCardsByRarityAsync(rarity);
            return Ok(new { success = true, data = cards, count = cards.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards by rarity {Rarity}", rarity);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get cards by price range
    /// </summary>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <returns>Cards within the price range</returns>
    [HttpGet("by-price")]
    public async Task<IActionResult> GetCardsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        try
        {
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
            {
                return BadRequest(new { success = false, message = "Invalid price range" });
            }

            var cards = await _cardService.GetCardsByPriceRangeAsync(minPrice, maxPrice);
            return Ok(new { success = true, data = cards, count = cards.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards by price range {MinPrice}-{MaxPrice}", minPrice, maxPrice);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get cards currently in stock
    /// </summary>
    /// <returns>Cards with stock quantity > 0</returns>
    [HttpGet("in-stock")]
    public async Task<IActionResult> GetCardsInStock()
    {
        try
        {
            var cards = await _cardService.GetCardsInStockAsync();
            return Ok(new { success = true, data = cards, count = cards.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards in stock");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Create a new card (Admin only)
    /// </summary>
    /// <param name="createCardDto">Card creation data</param>
    /// <returns>Created card</returns>
    [HttpPost]
    [Authorize] // This would typically require admin role
    public async Task<IActionResult> CreateCard([FromBody] CreateCardDto createCardDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
            }

            var card = await _cardService.CreateCardAsync(createCardDto);
            _logger.LogInformation("Card {CardName} created successfully", createCardDto.Name);
            
            return CreatedAtAction(nameof(GetCard), new { id = card.Id }, 
                new { success = true, data = card });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating card {CardName}", createCardDto.Name);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update an existing card (Admin only)
    /// </summary>
    /// <param name="id">Card ID</param>
    /// <param name="updateCardDto">Card update data</param>
    /// <returns>Updated card</returns>
    [HttpPut("{id}")]
    [Authorize] // This would typically require admin role
    public async Task<IActionResult> UpdateCard(int id, [FromBody] UpdateCardDto updateCardDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
            }

            var card = await _cardService.UpdateCardAsync(id, updateCardDto);
            
            if (card == null)
            {
                return NotFound(new { success = false, message = "Card not found" });
            }

            _logger.LogInformation("Card {CardId} updated successfully", id);
            return Ok(new { success = true, data = card });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card {CardId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Delete a card (Admin only - soft delete)
    /// </summary>
    /// <param name="id">Card ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id}")]
    [Authorize] // This would typically require admin role
    public async Task<IActionResult> DeleteCard(int id)
    {
        try
        {
            var success = await _cardService.DeleteCardAsync(id);
            
            if (!success)
            {
                return NotFound(new { success = false, message = "Card not found" });
            }

            _logger.LogInformation("Card {CardId} deleted successfully", id);
            return Ok(new { success = true, message = "Card deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting card {CardId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Check if a card is in stock
    /// </summary>
    /// <param name="id">Card ID</param>
    /// <param name="quantity">Required quantity (default: 1)</param>
    /// <returns>Stock availability</returns>
    [HttpGet("{id}/stock")]
    public async Task<IActionResult> CheckStock(int id, [FromQuery] int quantity = 1)
    {
        try
        {
            var inStock = await _cardService.IsCardInStockAsync(id, quantity);
            return Ok(new { success = true, inStock = inStock, cardId = id, requiredQuantity = quantity });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking stock for card {CardId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}