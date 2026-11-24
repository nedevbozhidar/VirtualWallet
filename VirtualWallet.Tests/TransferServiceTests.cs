using System.Net;
using System.Net.Http.Json;
using VirtualWallet.Business.DTOs.Transfers;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Implementations;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Repositories.Contracts;
using VirtualWallet.Data.Security;

namespace VirtualWallet.Tests;

public class TransferServiceTests
{
    [Fact]
    public async Task FundWalletFromCardAsync_ReturnsResponse_WhenRequestIsValid()
    {
        int userId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>();

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);

        TransferCreateDto createDto = new TransferCreateDto
        {
            CardId = 1,
            WalletId = 1,
            CheckNumber = "123",
            Amount = 100
        };
        
        var result = await service.FundWalletFromCardAsync(userId, createDto);
        
        Assert.Equal(100, result.Amount);
        Assert.Equal(200, wallets[0].Balance);
        Assert.Equal("Confirmed", result.Status);
        Assert.Equal("Approved", result.Message);
        Assert.Single(transfers);
    }
    
    [Fact]
    public async Task FundWalletFromCardAsync_ThrowsInvalidUserInputException_WhenAmountIsInvalid()
    {
        int userId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>();

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);

        TransferCreateDto createDto = new TransferCreateDto
        {
            CardId = 1,
            WalletId = 1,
            CheckNumber = "123",
            Amount = -100
        };
        
        await Assert.ThrowsAsync<InvalidUserInputException>(() 
            => service.FundWalletFromCardAsync(userId, createDto));
    }
    
    [Fact]
    public async Task FundWalletFromCardAsync_ThrowsInvalidUserInputException_WhenCheckNumberIsInvalid()
    {
        int userId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>();

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);

        TransferCreateDto createDto = new TransferCreateDto
        {
            CardId = 1,
            WalletId = 1,
            CheckNumber = "12",
            Amount = 100
        };
        
        await Assert.ThrowsAsync<InvalidUserInputException>(() 
            => service.FundWalletFromCardAsync(userId, createDto));
    }
    
    [Fact]
    public async Task FundWalletFromCardAsync_ThrowsEntityNotFoundException_WhenCardDoesNotExist()
    {
        int userId = 1;
        
        int missindCardId = 2;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>();

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);

        TransferCreateDto createDto = new TransferCreateDto
        {
            CardId = missindCardId,
            WalletId = 1,
            CheckNumber = "123",
            Amount = 100
        };
        
        await Assert.ThrowsAsync<EntityNotFoundException>(() 
            => service.FundWalletFromCardAsync(userId, createDto));
    }
    
    [Fact]
    public async Task FundWalletFromCardAsync_ThrowsUnauthorizedOperationException_WhenCardDoesntBelongToUser()
    {
        int userId = 1;
        int missindUserId = 2;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = missindUserId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>();

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);

        TransferCreateDto createDto = new TransferCreateDto
        {
            CardId = 1,
            WalletId = 1,
            CheckNumber = "123",
            Amount = 100
        };
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>(() 
            => service.FundWalletFromCardAsync(userId, createDto));
    }
    
    [Fact]
    public async Task FundWalletFromCardAsync_ThrowsEntityNotFoundException_WhenWalletDoesntExist()
    {
        int userId = 1;
        
        int missindWalletId = 2;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>();

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);

        TransferCreateDto createDto = new TransferCreateDto
        {
            CardId = 1,
            WalletId = missindWalletId,
            CheckNumber = "123",
            Amount = 100
        };
        
        await Assert.ThrowsAsync<EntityNotFoundException>(() 
            => service.FundWalletFromCardAsync(userId, createDto));
    }
    
    [Fact]
    public async Task FundWalletFromCardAsync_ThrowsUnauthorizedOperationException_WhenWalletDoesntBelongToUser()
    {
        int userId = 1;
        
        int missindUserId = 2;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, UserId = missindUserId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>();

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);

        TransferCreateDto createDto = new TransferCreateDto
        {
            CardId = 1,
            WalletId = 1,
            CheckNumber = "123",
            Amount = 100
        };
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>(() 
            => service.FundWalletFromCardAsync(userId, createDto));
    }
    
    [Fact]
    public async Task FundWalletFromCardAsync_ReturnsResponse_WhenCardIsDeclined()
    {
        int userId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>();

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = false,
                message = "Card issuer declined the payment"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);

        TransferCreateDto createDto = new TransferCreateDto
        {
            CardId = 1,
            WalletId = 1,
            CheckNumber = "123",
            Amount = 100
        };
        
        var result = await service.FundWalletFromCardAsync(userId, createDto);
        
        Assert.Equal(100, result.Amount);
        Assert.Equal(100, wallets[0].Balance);
        Assert.Equal("Card issuer declined the payment", result.Message);
        Assert.Equal("Declined", result.Status);
        Assert.Single(transfers);
        Assert.Equal(TransferStatus.Declined, transfers[0].Status);
    }

    [Fact]
    public async Task GetTransferHistoryAsync_ReturnsHistory()
    {
        int userId = 1;
        int walletId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = walletId, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>
        {
            new Transfer {Id = 1, Amount = 100, Card = cards[0], CardId = 1, Wallet =  wallets[0], WalletId = walletId}
        };

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);
        
        var result = await service.GetTransferHistoryAsync(userId, walletId);
        
        Assert.Single(result);
        Assert.Equal(1, result.First().TransferId);
        Assert.Equal(100, result.First().Amount);
    }
    
    [Fact]
    public async Task GetTransferHistoryAsync_ThrowsEntityNotFoundException_WhenWalletDoesntExist()
    {
        int userId = 1;
        int walletId = 1;
        int missingWalletId = 2;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = walletId, UserId = userId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>
        {
            new Transfer {Id = 1, Amount = 100, Card = cards[0], CardId = 1, Wallet =  wallets[0], WalletId = walletId}
        };

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.GetTransferHistoryAsync(userId, missingWalletId));
    }
    
    [Fact]
    public async Task GetTransferHistoryAsync_ThrowsUnauthorizedOperationException_WhenWalletDoesntBelongToUser()
    {
        int userId = 1;
        int missingUserId = 2;
        int walletId = 1;

        List<Card> cards = new List<Card>
        {
            new Card { Id = 1, UserId = userId, CardHolder = "User1", ProtectedCardNumber = "1111222233334444", ExpirationDate = DateTime.Now.AddYears(1)}
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = walletId, UserId = missingUserId, Name = "Wallet1", Balance = 100 }
        };

        List<Transfer> transfers = new List<Transfer>
        {
            new Transfer {Id = 1, Amount = 100, Card = cards[0], CardId = 1, Wallet =  wallets[0], WalletId = walletId}
        };

        FakeTransferRepository transferRepository = new FakeTransferRepository(transfers);
        FakeCardRepository cardRepository = new FakeCardRepository(cards);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeCardDataProtector cardDataProtector = new FakeCardDataProtector();

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                id = 1,
                isSuccess = true,
                message = "Approved"
            })
        };
        
        HttpClient client = new HttpClient(new FakeHttpMessageHandler(response));
        
        TransferService service = new TransferService(transferRepository, cardRepository, walletRepository, client, cardDataProtector);
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>(()
            => service.GetTransferHistoryAsync(userId, walletId));
    }

    public class FakeTransferRepository : ITransferRepository
    {
        private readonly List<Transfer> _transfers;
        
        public FakeTransferRepository(List<Transfer> transfers)
        {
            _transfers = transfers;
        }
        
        public async Task<Transfer?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_transfers.FirstOrDefault(t => t.Id == id));
        }

        public async Task<IEnumerable<Transfer>> GetByWalletIdAsync(int walletId)
        {
            return await Task.FromResult(_transfers.Where(t => t.WalletId == walletId));
        }

        public Task AddAsync(Transfer transfer)
        {
            transfer.Id = _transfers.Count + 1;
            _transfers.Add(transfer);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }

    public class FakeWalletRepository : IWalletRepository
    {
        private readonly List<Wallet> _wallets;

        public FakeWalletRepository(List<Wallet> wallets)
        {
            _wallets = wallets;
        }
        
        public async Task<Wallet?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_wallets.FirstOrDefault(t => t.Id == id));
        }

        public async Task<Wallet?> GetByIdWithDetailsAsync(int id)
        {
            return await Task.FromResult(_wallets.FirstOrDefault(w => w.Id == id));
        }

        public async Task<IEnumerable<Wallet>> GetByUserIdAsync(int userId)
        {
            return await Task.FromResult(_wallets.Where(w => w.UserId == userId));
        }

        public Task AddAsync(Wallet wallet)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Wallet wallet)
        {
            int index = _wallets.FindIndex(w => w.Id == wallet.Id);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"Wallet with id {wallet.Id} does not exist.");
            }
            
            _wallets[index] = wallet;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Wallet wallet)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateSavingsWalletWithTransfersAsync(Wallet sourceWallet, Wallet savingsWallet)
        {
            throw new NotImplementedException();
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
            return await Task.FromResult(_cards.FirstOrDefault(t => t.Id == id));
        }

        public async Task<IEnumerable<Card>> GetByUserIdAsync(int userId)
        {
            return await Task.FromResult(_cards.Where(t => t.UserId == userId));
        }

        public Task AddAsync(Card card)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Card card)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Card card)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
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

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        
        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}