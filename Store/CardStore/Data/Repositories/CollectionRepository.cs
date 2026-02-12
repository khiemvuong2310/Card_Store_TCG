using CardStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CardStore.Data.Repositories;

public class CollectionRepository : BaseRepository<Collection>, ICollectionRepository
{
    public CollectionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Collection>> GetCollectionsByUserAsync(int userId)
    {
        return await _dbSet
            .Include(c => c.Card)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.AcquiredDate)
            .ToListAsync();
    }

    public async Task<Collection?> GetUserCardCollectionAsync(int userId, int cardId)
    {
        return await _dbSet
            .Include(c => c.Card)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CardId == cardId);
    }

    public async Task<IEnumerable<Collection>> GetCollectionsByCardAsync(int cardId)
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.CardId == cardId)
            .ToListAsync();
    }

    public async Task<bool> AddToCollectionAsync(int userId, int cardId, int quantity)
    {
        var existingCollection = await GetUserCardCollectionAsync(userId, cardId);
        
        if (existingCollection != null)
        {
            existingCollection.Quantity += quantity;
            existingCollection.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newCollection = new Collection
            {
                UserId = userId,
                CardId = cardId,
                Quantity = quantity,
                AcquiredDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _dbSet.AddAsync(newCollection);
        }

        return true;
    }

    public async Task<bool> RemoveFromCollectionAsync(int userId, int cardId, int? quantity = null)
    {
        var collection = await GetUserCardCollectionAsync(userId, cardId);
        
        if (collection == null)
            return false;

        if (quantity.HasValue)
        {
            if (collection.Quantity <= quantity.Value)
            {
                _dbSet.Remove(collection);
            }
            else
            {
                collection.Quantity -= quantity.Value;
                collection.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // Remove all
            _dbSet.Remove(collection);
        }

        return true;
    }

    public async Task<bool> UpdateCollectionQuantityAsync(int userId, int cardId, int newQuantity)
    {
        var collection = await GetUserCardCollectionAsync(userId, cardId);
        
        if (collection == null)
            return false;

        if (newQuantity <= 0)
        {
            _dbSet.Remove(collection);
        }
        else
        {
            collection.Quantity = newQuantity;
            collection.UpdatedAt = DateTime.UtcNow;
        }

        return true;
    }

    public async Task<int> GetTotalCardsInCollectionAsync(int userId)
    {
        return await _dbSet
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity);
    }

    public async Task<decimal> GetCollectionValueAsync(int userId)
    {
        return await _dbSet
            .Include(c => c.Card)
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity * c.Card.Price);
    }
}