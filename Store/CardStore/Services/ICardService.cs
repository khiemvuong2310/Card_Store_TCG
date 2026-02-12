using CardStore.DTOs;
using CardStore.Models;

namespace CardStore.Services;

public interface ICardService
{
    Task<IEnumerable<CardDto>> GetAllCardsAsync();
    Task<CardDto?> GetCardByIdAsync(int id);
    Task<IEnumerable<CardDto>> GetActiveCardsAsync();
    Task<IEnumerable<CardDto>> GetCardsInStockAsync();
    Task<IEnumerable<CardDto>> GetCardsBySetAsync(string set);
    Task<IEnumerable<CardDto>> GetCardsByRarityAsync(string rarity);
    Task<IEnumerable<CardDto>> GetCardsByTypeAsync(string type);
    Task<IEnumerable<CardDto>> SearchCardsAsync(string searchTerm);
    Task<IEnumerable<CardDto>> GetCardsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<CardDto>> GetPagedCardsAsync(int pageNumber, int pageSize);
    Task<CardDto> CreateCardAsync(CreateCardDto createCardDto);
    Task<CardDto?> UpdateCardAsync(int id, UpdateCardDto updateCardDto);
    Task<bool> DeleteCardAsync(int id);
    Task<bool> IsCardInStockAsync(int cardId, int requiredQuantity = 1);
    Task<bool> UpdateStockAsync(int cardId, int quantity);
}