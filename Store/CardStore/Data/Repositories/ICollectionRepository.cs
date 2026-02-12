using CardStore.Models;

namespace CardStore.Data.Repositories;

public interface ICollectionRepository : IBaseRepository<Collection>
{
    Task<IEnumerable<Collection>> GetCollectionsByUserAsync(int userId);
    Task<Collection?> GetUserCardCollectionAsync(int userId, int cardId);
    Task<IEnumerable<Collection>> GetCollectionsByCardAsync(int cardId);
    Task<bool> AddToCollectionAsync(int userId, int cardId, int quantity);
    Task<bool> RemoveFromCollectionAsync(int userId, int cardId, int? quantity = null);
    Task<bool> UpdateCollectionQuantityAsync(int userId, int cardId, int newQuantity);
    Task<int> GetTotalCardsInCollectionAsync(int userId);
    Task<decimal> GetCollectionValueAsync(int userId);
}