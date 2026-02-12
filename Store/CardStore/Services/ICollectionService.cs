using CardStore.DTOs;

namespace CardStore.Services;

public interface ICollectionService
{
    Task<IEnumerable<CollectionDto>> GetUserCollectionAsync(int userId);
    Task<CollectionDto?> GetUserCardCollectionAsync(int userId, int cardId);
    Task<CollectionSummaryDto> GetCollectionSummaryAsync(int userId);
    Task<CollectionDto> AddToCollectionAsync(int userId, AddToCollectionDto addToCollectionDto);
    Task<bool> RemoveFromCollectionAsync(int userId, int cardId, int? quantity = null);
    Task<CollectionDto?> UpdateCollectionAsync(int userId, int cardId, UpdateCollectionDto updateCollectionDto);
    Task<decimal> GetCollectionValueAsync(int userId);
    Task<int> GetTotalCardsInCollectionAsync(int userId);
}