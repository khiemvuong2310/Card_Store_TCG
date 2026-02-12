using AutoMapper;
using CardStore.Data.Repositories;
using CardStore.DTOs;
using CardStore.Models;

namespace CardStore.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IMapper _mapper;

    public CardService(ICardRepository cardRepository, IMapper mapper)
    {
        _cardRepository = cardRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CardDto>> GetAllCardsAsync()
    {
        var cards = await _cardRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<CardDto?> GetCardByIdAsync(int id)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        return card == null ? null : _mapper.Map<CardDto>(card);
    }

    public async Task<IEnumerable<CardDto>> GetActiveCardsAsync()
    {
        var cards = await _cardRepository.GetActiveCardsAsync();
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsInStockAsync()
    {
        var cards = await _cardRepository.GetCardsInStockAsync();
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsBySetAsync(string set)
    {
        var cards = await _cardRepository.GetCardsBySetAsync(set);
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsByRarityAsync(string rarity)
    {
        var cards = await _cardRepository.GetCardsByRarityAsync(rarity);
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsByTypeAsync(string type)
    {
        var cards = await _cardRepository.GetCardsByTypeAsync(type);
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> SearchCardsAsync(string searchTerm)
    {
        var cards = await _cardRepository.SearchCardsAsync(searchTerm);
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var cards = await _cardRepository.GetCardsByPriceRangeAsync(minPrice, maxPrice);
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetPagedCardsAsync(int pageNumber, int pageSize)
    {
        var cards = await _cardRepository.GetPagedAsync(pageNumber, pageSize);
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<CardDto> CreateCardAsync(CreateCardDto createCardDto)
    {
        var card = _mapper.Map<Card>(createCardDto);
        var createdCard = await _cardRepository.AddAsync(card);
        await _cardRepository.SaveChangesAsync();
        
        return _mapper.Map<CardDto>(createdCard);
    }

    public async Task<CardDto?> UpdateCardAsync(int id, UpdateCardDto updateCardDto)
    {
        var existingCard = await _cardRepository.GetByIdAsync(id);
        if (existingCard == null)
            return null;

        _mapper.Map(updateCardDto, existingCard);
        existingCard.UpdatedAt = DateTime.UtcNow;
        
        var updatedCard = await _cardRepository.UpdateAsync(existingCard);
        await _cardRepository.SaveChangesAsync();
        
        return _mapper.Map<CardDto>(updatedCard);
    }

    public async Task<bool> DeleteCardAsync(int id)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        if (card == null)
            return false;

        // Soft delete - just mark as inactive
        card.IsActive = false;
        card.UpdatedAt = DateTime.UtcNow;
        
        await _cardRepository.UpdateAsync(card);
        await _cardRepository.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> IsCardInStockAsync(int cardId, int requiredQuantity = 1)
    {
        return await _cardRepository.IsCardInStockAsync(cardId, requiredQuantity);
    }

    public async Task<bool> UpdateStockAsync(int cardId, int quantity)
    {
        var success = await _cardRepository.UpdateStockAsync(cardId, quantity);
        if (success)
            await _cardRepository.SaveChangesAsync();
        
        return success;
    }
}