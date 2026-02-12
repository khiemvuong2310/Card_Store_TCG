using CardStore.Models;

namespace CardStore.Data.Repositories;

public interface ICardRepository : IBaseRepository<Card>
{
    Task<IEnumerable<Card>> GetCardsBySetAsync(string set);
    Task<IEnumerable<Card>> GetCardsByRarityAsync(string rarity);
    Task<IEnumerable<Card>> GetCardsByTypeAsync(string type);
    Task<IEnumerable<Card>> GetActiveCardsAsync();
    Task<IEnumerable<Card>> GetCardsInStockAsync();
    Task<IEnumerable<Card>> SearchCardsAsync(string searchTerm);
    Task<IEnumerable<Card>> GetCardsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<bool> IsCardInStockAsync(int cardId, int requiredQuantity = 1);
    Task<bool> UpdateStockAsync(int cardId, int quantity);
}