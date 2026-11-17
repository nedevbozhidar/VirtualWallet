using VirtualWallet.Business.DTOs.Cards;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Implementations;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;
using VirtualWallet.Data.Security;

namespace VirtualWallet.Tests;

public class CardServiceTests
{
    [Fact]
    public async Task GetUserCardsAsync_ReturnsCardsForUser()
    {
        int userId = 1;
        List<Card> cards = new List<Card>
        {
            new Card {Id = 1, CardHolder = "User1", UserId =  userId,},
            new Card {Id = 2, CardHolder = "User2", UserId =  userId}
        };

        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);
        
        var result = await service.GetUserCardsAsync(userId);
        
        Assert.Equal(2, result.Count());
        Assert.Equal("User1", result.First().CardHolder);
        Assert.Equal("User2", result.Last().CardHolder);
    }

    [Fact]
    public async Task GetUserCardsAsync_ReturnsEmptyCardsForUser()
    {
        int userId = 1;
        List<Card> cards = new List<Card>();
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);
        
        var result = await service.GetUserCardsAsync(userId);
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCardsForUser()
    {
        int userId = 1;
        int cardId = 1;
        
        List<Card> cards = new List<Card>
        {
            new Card {Id = 1, CardHolder = "User1", UserId =  userId,},
            new Card {Id = 2, CardHolder = "User2", UserId =  userId}
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);
        
        var result = await service.GetByIdAsync(cardId, userId);
        
        Assert.Equal(cardId, result.Id);
        Assert.Equal("User1", result.CardHolder);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsEntityNotFoundException_WhenCardDoesntExist()
    {
        int userId = 1;
        int cardId = 3;
        
        List<Card> cards = new List<Card>
        { 
            new Card {Id = 1, CardHolder = "User1", UserId =  userId,},
            new Card {Id = 2, CardHolder = "User2", UserId =  userId}
        };
       
        FakeCardRepository cardRepository = new FakeCardRepository(cards); 
        CardService service = new CardService(cardRepository, null!, null!);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(() 
            => service.GetByIdAsync(cardId, userId));
    }
    
    [Fact]
    public async Task GetByIdAsync_ThrowsUnauthorizedOperationException_WhenUserDoesntOwnCard()
    {
        int userId = 1;
        int cardId = 1;
        
        List<Card> cards = new List<Card>
        { 
            new Card {Id = 1, CardHolder = "User1", UserId =  2,},
            new Card {Id = 2, CardHolder = "User2", UserId =  2}
        };
       
        FakeCardRepository cardRepository = new FakeCardRepository(cards); 
        CardService service = new CardService(cardRepository, null!, null!);
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>(() 
            => service.GetByIdAsync(cardId, userId));
    }
    
    [Fact]
    public async Task AddCardAsync_ReturnsCard_WhenCardIsCreated()
    {
        int userId = 1;

        List<User> users = new List<User>
        {
            new User {Id = userId}
        };

        FakeCardRepository cardRepository = new FakeCardRepository(new List<Card>());
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCardDataProtector protector = new FakeCardDataProtector();
        CardService service = new CardService(cardRepository, userRepository, protector);

        CardCreateDto cardCreateDto = new CardCreateDto {CardHolder = "User1", CardNumber = "1234123412341234", ExpirationDate = DateTime.Now.AddYears(1)};

        var result = await service.AddCardAsync(userId, cardCreateDto);
        var savedCard = await cardRepository.GetByUserIdAsync(userId);
        
        Assert.Equal("User1", result.CardHolder);
        var storedCard = Assert.Single(savedCard);
        Assert.Equal("protected: 1234123412341234", storedCard.ProtectedCardNumber);
        Assert.Equal("**** **** **** 1234",  result.MaskedCardNumber);
    }
    
    [Fact]
    public async Task AddCardAsync_ThrowsInvalidUserInputException_WhenCardNumberIsInvalid()
    {
        int userId = 1;

        List<User> users = new List<User>
        {
            new User {Id = userId}
        };

        FakeCardRepository cardRepository = new FakeCardRepository(new List<Card>());
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCardDataProtector protector = new FakeCardDataProtector();
        CardService service = new CardService(cardRepository, userRepository, protector);

        CardCreateDto cardCreateDto = new CardCreateDto {CardHolder = "User1", CardNumber = "123412341234123", ExpirationDate = DateTime.Now.AddYears(1)};

        await  Assert.ThrowsAsync<InvalidUserInputException>(()
            => service.AddCardAsync(userId, cardCreateDto));
    }
    
    [Fact]
    public async Task AddCardAsync_ThrowsEntityNotFoundException_WhenUserIdIsNotFound()
    {
        int userId = 1;
        int missingUserId = 2;

        List<User> users = new List<User>
        {
            new User {Id = userId}
        };

        FakeCardRepository cardRepository = new FakeCardRepository(new List<Card>());
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCardDataProtector protector = new FakeCardDataProtector();
        CardService service = new CardService(cardRepository, userRepository, protector);

        CardCreateDto cardCreateDto = new CardCreateDto {CardHolder = "User1", CardNumber = "1234123412341234", ExpirationDate = DateTime.Now.AddYears(1)};

        await  Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.AddCardAsync(missingUserId, cardCreateDto));
    }
    
    [Fact]
    public async Task AddCardAsync_ThrowsInvalidUserInputException_WhenExpirationDateIsInvalid()
    {
        int userId = 1;

        List<User> users = new List<User>
        {
            new User {Id = userId}
        };

        FakeCardRepository cardRepository = new FakeCardRepository(new List<Card>());
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCardDataProtector protector = new FakeCardDataProtector();
        CardService service = new CardService(cardRepository, userRepository, protector);

        CardCreateDto cardCreateDto = new CardCreateDto {CardHolder = "User1", CardNumber = "1234123412341234", ExpirationDate = DateTime.MinValue};

        await Assert.ThrowsAsync<InvalidUserInputException>(()
            => service.AddCardAsync(userId, cardCreateDto));
    }
    
    [Fact]
    public async Task DeleteCardAsync_DeletesCard_WhenCardExists()
    {
        int userId = 1;
        int cardId = 1;
        
        List<Card> cards = new List<Card>
        { 
            new Card {Id = cardId, CardHolder = "User1", UserId =  userId,},
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);

        await service.DeleteCardAsync(cardId, userId);
        var savedCards = await cardRepository.GetByUserIdAsync(userId);
        Assert.Empty(savedCards);
    }
    
    [Fact]
    public async Task DeleteCardAsync_ThrowsEntityNotFoundException_WhenCardDoesntExist()
    {
        int userId = 1;
        int cardId = 1;
        
        List<Card> cards = new List<Card>
        { 
            new Card {Id = 2, CardHolder = "User1", UserId =  userId,},
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);

        await Assert.ThrowsAsync<EntityNotFoundException>(() 
            => service.DeleteCardAsync(cardId, userId));
    }
    
    [Fact]
    public async Task DeleteCardAsync_UnauthorizedOperationException_WhenCardDoesntBelongToUser()
    {
        int userId = 1;
        int cardId = 1;
        
        List<Card> cards = new List<Card>
        { 
            new Card {Id = cardId, CardHolder = "User1", UserId =  2,},
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);

        await Assert.ThrowsAsync<UnauthorizedOperationException>(() 
            => service.DeleteCardAsync(cardId, userId));
    }

    [Fact]
    public async Task SetPrimaryCardAsync_SetsPrimaryCard_WhenCardExists()
    {
        int userId = 1;
        int cardId = 1;
        
        List<Card> cards = new List<Card>
        { 
            new Card {Id = cardId, CardHolder = "User1", UserId =  userId,},
            new Card {Id = 2, CardHolder = "User2", UserId =  userId}
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);
        
        await service.SetPrimaryCardAsync(cardId, userId);
        var changedCard = await cardRepository.GetByIdAsync(cardId);
        
        Assert.Equal(true, changedCard.IsPrimary);
    }
    
    [Fact]
    public async Task SetPrimaryCardAsync_ThrowsEntityNotFoundException_WhenCardDoesntExists()
    {
        int userId = 1;
        int missingCardId = 1;
        
        List<Card> cards = new List<Card>
        { 
            new Card {Id = 2, CardHolder = "User1", UserId =  userId,},
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.SetPrimaryCardAsync(missingCardId, userId));
    }

    [Fact]
    public async Task UpdateCardAsync_UpdatesCard_WhenCardExists()
    {
        int userId = 1;
        int cardId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = cardId, CardHolder = "User1", UserId = userId },
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);

        CardUpdateDto cardUpdateDto = new CardUpdateDto { CardHolder = "User", ExpirationDate =  DateTime.Now.AddYears(1) };
        
        var result = await service.UpdateCardAsync(cardId, userId, cardUpdateDto);
        var savedCard = await cardRepository.GetByIdAsync(cardId);
        
        Assert.Equal(cardUpdateDto.CardHolder, result.CardHolder);
        Assert.Equal(cardUpdateDto.CardHolder, savedCard.CardHolder);
        Assert.Equal(cardId, result.Id);
    }
    
    [Fact]
    public async Task UpdateCardAsync_ThrowsEntityNotFoundException_WhenCardDoesntExist()
    {
        int userId = 1;
        int missingCardId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 2, CardHolder = "User1", UserId = userId },
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);

        CardUpdateDto cardUpdateDto = new CardUpdateDto { CardHolder = "User", ExpirationDate =  DateTime.Now.AddYears(1) };

        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            service.UpdateCardAsync(missingCardId, userId, cardUpdateDto));
    }
    
    [Fact]
    public async Task UpdateCardAsync_ThrowsUnauthorizedOperationException_WhenCardDoesntBelongToUser()
    {
        int userId = 1;
        int cardId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = cardId, CardHolder = "User1", UserId = 2 },
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);

        CardUpdateDto cardUpdateDto = new CardUpdateDto { CardHolder = "User", ExpirationDate =  DateTime.Now.AddYears(1) };

        await Assert.ThrowsAsync<UnauthorizedOperationException>(() => 
            service.UpdateCardAsync(cardId, userId, cardUpdateDto));
    }
    
    [Fact]
    public async Task UpdateCardAsync_ThrowsInvalidUserInputException_WhenCardExpDateIsInvalid()
    {
        int userId = 1;
        int cardId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = cardId, CardHolder = "User1", UserId = userId },
        };
        
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        CardService service = new CardService(cardRepository, null!, null!);

        CardUpdateDto cardUpdateDto = new CardUpdateDto { CardHolder = "User", ExpirationDate =  DateTime.Now.AddYears(-1) };

        await Assert.ThrowsAsync<InvalidUserInputException>(() => 
            service.UpdateCardAsync(cardId, userId, cardUpdateDto));
    }

    public class FakeCardDataProtector : ICardDataProtector
    {
        public string Protect(string cardNumber)
        {
            return $"protected: {cardNumber}";
        }

        public string Unprotect(string protectedCardNumber)
        {
            return protectedCardNumber;
        }
    }
    public class FakeCardRepository : ICardRepository
    {
        private readonly List<Card> _cards;

        public FakeCardRepository(List<Card> cards)
        {
            _cards = cards;
        }
        
        public async Task<Card?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_cards.FirstOrDefault(c => c.Id == id));
        }

        public async Task<IEnumerable<Card>> GetByUserIdAsync(int userId)
        {
            return await Task.FromResult(_cards.Where(x => x.UserId == userId));
        }

        public Task AddAsync(Card card)
        {
            _cards.Add(card);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Card card)
        {
            int index = _cards.FindIndex(c => c.Id == card.Id);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"Card with  id {card.Id} does not exist.");
            }
            
            _cards[index] = card;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Card card)
        {
            _cards.Remove(card);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
    
    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users;
    
        public FakeUserRepository(List<User> users)
        {
            _users = users;
        }
    
        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task<User?> GetByIdWithDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}