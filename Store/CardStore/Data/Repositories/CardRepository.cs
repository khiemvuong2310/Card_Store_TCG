using CardStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CardStore.Data.Repositories;

public class CardRepository : BaseRepository<Card>, ICardRepository
{
    public CardRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Card>> GetCardsBySetAsync(string set)
    {
        return await _dbSet
            .Where(c => c.Set == set && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetCardsByRarityAsync(string rarity)
    {
        return await _dbSet
            .Where(c => c.Rarity == rarity && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetCardsByTypeAsync(string type)
    {
        return await _dbSet
            .Where(c => c.Type == type && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetActiveCardsAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetCardsInStockAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive && c.StockQuantity > 0)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> SearchCardsAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(c => c.IsActive && 
                       (c.Name.ToLower().Contains(lowerSearchTerm) || 
                        c.Description.ToLower().Contains(lowerSearchTerm) ||
                        c.Set.ToLower().Contains(lowerSearchTerm) ||
                        (c.Type != null && c.Type.ToLower().Contains(lowerSearchTerm))))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetCardsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Where(c => c.IsActive && c.Price >= minPrice && c.Price <= maxPrice)
            .OrderBy(c => c.Price)
            .ToListAsync();
    }

    public async Task<bool> IsCardInStockAsync(int cardId, int requiredQuantity = 1)
    {
        var card = await _dbSet.FindAsync(cardId);
        return card != null && card.IsActive && card.StockQuantity >= requiredQuantity;
    }

    public async Task<bool> UpdateStockAsync(int cardId, int quantity)
    {
        var card = await _dbSet.FindAsync(cardId);
        if (card == null || !card.IsActive)
            return false;

        if (card.StockQuantity + quantity < 0)
            return false;

        card.StockQuantity += quantity;
        card.UpdatedAt = DateTime.UtcNow;
        
        return true;
    }
}