using AutoMapper;
using CardStore.Data.Repositories;
using CardStore.DTOs;
using CardStore.Models;

namespace CardStore.Services;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IMapper _mapper;

    public CollectionService(
        ICollectionRepository collectionRepository, 
        IUserRepository userRepository, 
        ICardRepository cardRepository, 
        IMapper mapper)
    {
        _collectionRepository = collectionRepository;
        _userRepository = userRepository;
        _cardRepository = cardRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CollectionDto>> GetUserCollectionAsync(int userId)
    {
        var collections = await _collectionRepository.GetCollectionsByUserAsync(userId);
        return collections.Select(c => new CollectionDto
        {
            Id = c.Id,
            UserId = c.UserId,
            Username = c.User?.Username ?? "",
            CardId = c.CardId,
            CardName = c.Card.Name,
            CardSet = c.Card.Set,
            CardRarity = c.Card.Rarity,
            CardPrice = c.Card.Price,
            CardImageUrl = c.Card.ImageUrl,
            Quantity = c.Quantity,
            TotalValue = c.Quantity * c.Card.Price,
            AcquiredDate = c.AcquiredDate,
            Notes = c.Notes,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }

    public async Task<CollectionDto?> GetUserCardCollectionAsync(int userId, int cardId)
    {
        var collection = await _collectionRepository.GetUserCardCollectionAsync(userId, cardId);
        
        if (collection == null)
            return null;

        return new CollectionDto
        {
            Id = collection.Id,
            UserId = collection.UserId,
            Username = collection.User?.Username ?? "",
            CardId = collection.CardId,
            CardName = collection.Card.Name,
            CardSet = collection.Card.Set,
            CardRarity = collection.Card.Rarity,
            CardPrice = collection.Card.Price,
            CardImageUrl = collection.Card.ImageUrl,
            Quantity = collection.Quantity,
            TotalValue = collection.Quantity * collection.Card.Price,
            AcquiredDate = collection.AcquiredDate,
            Notes = collection.Notes,
            CreatedAt = collection.CreatedAt,
            UpdatedAt = collection.UpdatedAt
        };
    }

    public async Task<CollectionSummaryDto> GetCollectionSummaryAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var collections = await _collectionRepository.GetCollectionsByUserAsync(userId);
        
        var totalCards = collections.Sum(c => c.Quantity);
        var uniqueCards = collections.Count();
        var totalValue = collections.Sum(c => c.Quantity * c.Card.Price);
        var lastUpdated = collections.Any() ? collections.Max(c => c.UpdatedAt) : DateTime.MinValue;

        return new CollectionSummaryDto
        {
            UserId = userId,
            Username = user?.Username ?? "",
            TotalCards = totalCards,
            UniqueCards = uniqueCards,
            TotalValue = totalValue,
            LastUpdated = lastUpdated
        };
    }

    public async Task<CollectionDto> AddToCollectionAsync(int userId, AddToCollectionDto addToCollectionDto)
    {
        // Verify card exists and is active
        var card = await _cardRepository.GetByIdAsync(addToCollectionDto.CardId);
        if (card == null || !card.IsActive)
            throw new InvalidOperationException("Card not found or not active");

        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
            throw new InvalidOperationException("User not found or not active");

        await _collectionRepository.AddToCollectionAsync(userId, addToCollectionDto.CardId, addToCollectionDto.Quantity);
        
        // Update notes if provided
        var collection = await _collectionRepository.GetUserCardCollectionAsync(userId, addToCollectionDto.CardId);
        if (collection != null && !string.IsNullOrWhiteSpace(addToCollectionDto.Notes))
        {
            collection.Notes = addToCollectionDto.Notes;
            collection.UpdatedAt = DateTime.UtcNow;
        }

        await _collectionRepository.SaveChangesAsync();

        // Return the updated collection
        var updatedCollection = await GetUserCardCollectionAsync(userId, addToCollectionDto.CardId);
        return updatedCollection!;
    }

    public async Task<bool> RemoveFromCollectionAsync(int userId, int cardId, int? quantity = null)
    {
        var success = await _collectionRepository.RemoveFromCollectionAsync(userId, cardId, quantity);
        
        if (success)
            await _collectionRepository.SaveChangesAsync();
        
        return success;
    }

    public async Task<CollectionDto?> UpdateCollectionAsync(int userId, int cardId, UpdateCollectionDto updateCollectionDto)
    {
        var collection = await _collectionRepository.GetUserCardCollectionAsync(userId, cardId);
        
        if (collection == null)
            return null;

        collection.Quantity = updateCollectionDto.Quantity;
        collection.Notes = updateCollectionDto.Notes;
        collection.UpdatedAt = DateTime.UtcNow;

        if (updateCollectionDto.Quantity <= 0)
        {
            await _collectionRepository.DeleteAsync(collection);
        }

        await _collectionRepository.SaveChangesAsync();

        if (updateCollectionDto.Quantity <= 0)
            return null;

        return await GetUserCardCollectionAsync(userId, cardId);
    }

    public async Task<decimal> GetCollectionValueAsync(int userId)
    {
        return await _collectionRepository.GetCollectionValueAsync(userId);
    }

    public async Task<int> GetTotalCardsInCollectionAsync(int userId)
    {
        return await _collectionRepository.GetTotalCardsInCollectionAsync(userId);
    }
}