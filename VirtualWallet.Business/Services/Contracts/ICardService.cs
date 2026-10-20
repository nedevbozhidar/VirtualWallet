using VirtualWallet.Business.DTOs.Cards;

namespace VirtualWallet.Business.Services.Contracts;

public interface ICardService
{
    Task<CardResponseDto> AddCardAsync(int userId, CardCreateDto cardDto);
    Task<IEnumerable<CardResponseDto>> GetUserCardsAsync(int userId);
    Task<CardResponseDto> GetByIdAsync(int cardId, int userId);
    Task DeleteCardAsync(int cardId, int userId);
    Task SetPrimaryCardAsync(int cardId, int userId);
}
