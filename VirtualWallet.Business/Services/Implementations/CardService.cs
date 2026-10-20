using VirtualWallet.Business.DTOs.Cards;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;
using VirtualWallet.Data.Security;

namespace VirtualWallet.Business.Services.Implementations;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICardDataProtector _cardDataProtector;

    public CardService(
        ICardRepository cardRepository,
        IUserRepository userRepository,
        ICardDataProtector cardDataProtector)
    {
        _cardRepository = cardRepository;
        _userRepository = userRepository;
        _cardDataProtector = cardDataProtector;
    }

    public async Task<CardResponseDto> AddCardAsync(int userId, CardCreateDto cardDto)
    {
        if (string.IsNullOrEmpty(cardDto.CardNumber)
            || cardDto.CardNumber.Length != 16
            || cardDto.CardNumber.Any(character => !char.IsDigit(character)))
        {
            throw new InvalidUserInputException("Card number must be exactly 16 digits.");
        }

        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new EntityNotFoundException($"User with ID {userId} was not found.");

        if (cardDto.ExpirationDate <= DateTime.UtcNow)
        {
            throw new InvalidUserInputException("The card expiration date must be in the future.");
        }

        var existingCards = await _cardRepository.GetByUserIdAsync(userId);
        var isFirstCard = !existingCards.Any();

        var card = new Card
        {
            ProtectedCardNumber = _cardDataProtector.Protect(cardDto.CardNumber),
            LastFour = cardDto.CardNumber[^4..],
            ExpirationDate = cardDto.ExpirationDate,
            CardHolder = cardDto.CardHolder,
            IsPrimary = isFirstCard,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _cardRepository.AddAsync(card);

        return MapToDto(card);
    }

    public async Task<IEnumerable<CardResponseDto>> GetUserCardsAsync(int userId)
    {
        var cards = await _cardRepository.GetByUserIdAsync(userId);
        return cards.Select(MapToDto);
    }

    public async Task<CardResponseDto> GetByIdAsync(int cardId, int userId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId)
                   ?? throw new EntityNotFoundException($"Card with ID {cardId} was not found.");

        if (card.UserId != userId)
        {
            throw new UnauthorizedOperationException("You do not have permission to view this card.");
        }

        return MapToDto(card);
    }

    public async Task DeleteCardAsync(int cardId, int userId)
    {
        var card = await _cardRepository.GetByIdAsync(cardId)
                   ?? throw new EntityNotFoundException($"Card with ID {cardId} was not found.");

        if (card.UserId != userId)
        {
            throw new UnauthorizedOperationException("You do not have permission to delete this card.");
        }

        await _cardRepository.DeleteAsync(card);
    }

    public async Task SetPrimaryCardAsync(int cardId, int userId)
    {
        var userCards = (await _cardRepository.GetByUserIdAsync(userId)).ToList();

        var selectedCard = userCards.FirstOrDefault(card => card.Id == cardId);
        if (selectedCard is null)
        {
            throw new EntityNotFoundException($"Card with ID {cardId} was not found.");
        }

        foreach (var card in userCards)
        {
            card.IsPrimary = card.Id == cardId;
            await _cardRepository.UpdateAsync(card);
        }
    }

    private static CardResponseDto MapToDto(Card card)
    {
        return new CardResponseDto
        {
            Id = card.Id,
            MaskedCardNumber = $"**** **** **** {card.LastFour}",
            ExpirationDate = card.ExpirationDate,
            CardHolder = card.CardHolder,
            IsPrimary = card.IsPrimary,
            UserId = card.UserId,
            CreatedAt = card.CreatedAt
        };
    }
}
