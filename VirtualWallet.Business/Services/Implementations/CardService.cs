using VirtualWallet.Business.DTOs.Cards;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Business.Services.Implementations;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;

    public CardService(ICardRepository cardRepository, IUserRepository userRepository)
    {
        _cardRepository = cardRepository;
        _userRepository = userRepository;
    }

    public async Task<CardResponseDto> AddCardAsync(int userId, CardCreateDto cardDto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new EntityNotFoundException($"User with ID {userId} was not found.");

        if (cardDto.ExpirationDate <= DateTime.UtcNow)
        {
            throw new InvalidUserInputException("The card expiration date must be in the future.");
        }

        if (await _cardRepository.ExistsByCardNumberAsync(cardDto.CardNumber))
        {
            throw new DuplicateEntityException("A card with this number has already been registered.");
        }

        var existingCards = await _cardRepository.GetByUserIdAsync(userId);
        var isFirstCard = !existingCards.Any();

        var card = new Card
        {
            CardNumber = cardDto.CardNumber,
            ExpirationDate = cardDto.ExpirationDate,
            CardHolder = cardDto.CardHolder,
            CheckNumber = cardDto.CheckNumber,
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

    private static CardResponseDto MapToDto(Card card)
    {
        var lastFour = card.CardNumber.Length >= 4
            ? card.CardNumber[^4..]
            : card.CardNumber;

        return new CardResponseDto
        {
            Id = card.Id,
            CardNumber = card.CardNumber,
            MaskedCardNumber = $"**** **** **** {lastFour}",
            ExpirationDate = card.ExpirationDate,
            CardHolder = card.CardHolder,
            IsPrimary = card.IsPrimary,
            UserId = card.UserId,
            CreatedAt = card.CreatedAt
        };
    }
}
