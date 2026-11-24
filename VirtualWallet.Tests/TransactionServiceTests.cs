using VirtualWallet.Business.DTOs.Transactions;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Implementations;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Tests;

public class TransactionServiceTests
{
    [Fact]
    public async Task PreviewTransactionAsync_ReturnsPreview_WhenRequestIsValid()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2" },
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, null!);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test"};
        
        var result = await service.PreviewTransactionAsync(senderUserId, createDto);
        
        Assert.Equal(100, result.Amount);
        Assert.Equal("User2", result.RecipientUsername);
        Assert.Equal("Test",result.Description);
        Assert.Equal("Test1", result.SenderWalletName);
    }
    
    
    [Fact]
    public async Task PreviewTransactionAsync_ThrowsEntityNotFoundException_WhenSenderUserIdDoesNotExist()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2" },
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = 3 },
            new User { Username = "User2", Id = recipientUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, null!);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test"};
        
        await Assert.ThrowsAsync<EntityNotFoundException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task PreviewTransactionAsync_ThrowsEntityNotFoundException_WhenSenderWalletDoesNotExist()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 0, UserId = senderUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2" },
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, null!);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test"};
        
        await Assert.ThrowsAsync<EntityNotFoundException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }

    [Fact]
    public async Task PreviewTransactionAsync_ThrowsUnauthorizedOperationException_WhenSenderUserDoesNotOwnWallet()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = recipientUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2" },
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, null!);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test"};
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task PreviewTransactionAsync_ThrowsBlockedAccountException_WhenSenderAccountIsBlocked()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2" },
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId, IsBlocked =  true },
            new User { Username = "User2", Id = recipientUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, null!);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test"};
        
        await Assert.ThrowsAsync<BlockedAccountException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task PreviewTransactionAsync_ThrowsEntityNotFoundException_WhenRecipientIdentifierIsInvalid()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId},
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, null!);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "missing-recipient", SenderWalletId = senderWalletId, Amount = 100, Description = "Test"};
        
        await Assert.ThrowsAsync<EntityNotFoundException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task PreviewTransactionAsync_ThrowsInvalidUserInputException_WhenSenderUserIsTheSameAsRecipient()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2" },
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = senderUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, null!);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test"};
        
        await Assert.ThrowsAsync<InvalidUserInputException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task PreviewTransactionAsync_ThrowsEntityNotFoundException_WhenCategoryDoesntExist()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId},
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test", CategoryId = 1};
        
        await Assert.ThrowsAsync<EntityNotFoundException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task PreviewTransactionAsync_ThrowsUnauthorizedOperationException_WhenCategoryDoesntBelongToUser()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" },
            new Wallet { Id = recipientWalletId, UserId = recipientUserId},
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Category1", UserId =  recipientUserId },
            new Category { Id = 2, Name = "Category2" },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test", CategoryId = 1};
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task PreviewTransactionAsync_ThrowsEntityNotFoundException_WhenRecipientDoesntHaveWallet()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" }
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(new List<Transaction>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto { RecipientIdentifier = "User2", SenderWalletId = senderWalletId, Amount = 100, Description = "Test"};
        
        await Assert.ThrowsAsync<EntityNotFoundException>
            (() => service.PreviewTransactionAsync(senderUserId, createDto));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTransaction_WhenTransactionExists()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" }
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        var result = await service.GetByIdAsync(transactionId, senderUserId, false);
        Assert.Equal(transactionId, result.Id);
        Assert.Equal(100, result.Amount);
        Assert.Equal("Test", result.Description);
    }
    
    [Fact]
    public async Task GetByIdAsync_ThrowsEntityNotFoundException_WhenTransactionDoesntExist()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" }
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = 2, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        await Assert.ThrowsAsync<EntityNotFoundException>
            (() => service.GetByIdAsync(transactionId, senderUserId, false));
    }
    
    [Fact]
    public async Task GetByIdAsync_ThrowsUnauthorizedOperationException_WhenTransactionDoesntBelongToUser()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        
        int transactionId = 1;
        int unauthorizedUserId = 10;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" }
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        await Assert.ThrowsAsync<UnauthorizedOperationException>
            (() => service.GetByIdAsync(transactionId, unauthorizedUserId, false));
    }
    
    [Fact]
    public async Task GetFilteredTransactionsAsync_ReturnsTransactions_WhenTransactionExists()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1" }
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = transactionId,
                SenderUserId = senderUserId,
                SenderWalletId = senderWalletId,
                Amount = 100,
                Description = "Test",
                CategoryId = 1
            },
            new Transaction
            {
                Id = 2,
                SenderUserId = 3,
                SenderWalletId = 30,
                RecipientUserId = 4,
                RecipientWalletId = 40,
                Amount = 50,
                Description = "Does not match the requested wallet or user"
            }
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionFilterDto filterDto = new TransactionFilterDto { WalletId = senderWalletId, UserId = senderUserId };
        
        var result = await service.GetFilteredTransactionsAsync(filterDto);
        var matchingTransaction = Assert.Single(result);
        Assert.Equal(transactionId, matchingTransaction.Id);
    }

    [Fact]
    public async Task ExecuteTransactionAsync_ReturnsTransaction_WhenTransactionIsValid()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1", Balance = 1000},
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = senderWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test"
        };
        
        var result = await service.ExecuteTransactionAsync(senderUserId, createDto);
        
        Assert.Equal(100, result.Amount);
        Assert.Equal(900, wallets[0].Balance);
        Assert.Equal(100, wallets[1].Balance);
        Assert.Equal(2, result.Id);
        Assert.Equal("Test", result.Description);
        Assert.Equal("Confirmed", result.Status);
    }
    
    [Fact]
    public async Task ExecuteTransactionAsync_ThrowsInsufficientFundsException_WhenSenderWalletBalanceIsNotEnough()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1", Balance = 0},
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = senderWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test"
        };
        
        await Assert.ThrowsAsync<InsufficientFundsException>(() 
            => service.ExecuteTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task ExecuteTransactionAsync_ThrowsEntityNotFoundException_WhenSenderUserIsNotFound()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        int missingUserId = 3;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1", Balance = 1000},
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = senderWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test"
        };
        
        await  Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.ExecuteTransactionAsync(missingUserId, createDto));
    }
    
    [Fact]
    public async Task ExecuteTransactionAsync_ThrowsBlockedAccountException_WhenSenderUserAccountIsBlocked()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1", Balance = 1000},
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId, IsBlocked = true},
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = senderWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test"
        };
        
        await Assert.ThrowsAsync<BlockedAccountException>(()
            => service.ExecuteTransactionAsync(senderUserId, createDto));
    }

    [Fact]
    public async Task ExecuteTransactionAsync_ThrowsEntityNotFoundException_WhenSenderWalletIsNotFound()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;
        int missingWalletId = 30;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderUserId, UserId = senderUserId, Name = "Test1", Balance = 1000},
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = missingWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test"
        };
        
        await Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.ExecuteTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task ExecuteTransactionAsync_ThrowsUnauthorizedOperationException_WhenUserDoesNotOwnSenderWallet()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = recipientUserId, Name = "Test1", Balance = 1000},
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = recipientWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test"
        };
        
        await  Assert.ThrowsAsync<UnauthorizedOperationException>(()
            => service.ExecuteTransactionAsync(senderUserId, createDto));
    }

    [Fact]
    public async Task ExecuteTransactionAsync_ThrowsEntityNotFoundException_WhenCategoryWasNotFound()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1", Balance = 1000},
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = senderWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test",
            CategoryId = 1
        };
        
        await  Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.ExecuteTransactionAsync(senderUserId, createDto));
    }
    
    [Fact]
    public async Task ExecuteTransactionAsync_ThrowsUnauthorizedOperationException_WhenCategoryDoesntBelongToUser()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        int missingUserId = 3;
        
        int senderWalletId = 10;
        int recipientWalletId = 20;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1", Balance = 1000},
            new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test", CategoryId = 1 },
        };

        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, UserId = missingUserId },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = senderWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test",
            CategoryId = 1
        };
        
        await  Assert.ThrowsAsync<UnauthorizedOperationException>(()
            => service.ExecuteTransactionAsync(senderUserId, createDto));
    }

    [Fact]
    public async Task ExecuteTransactionAsync_ThrowsEntityNotFoundException_WhenRecipientDoesntHaveActiveWallet()
    {
        int senderUserId = 1;
        int recipientUserId = 2;
        
        int senderWalletId = 10;
        //int recipientWalletId = 20;
        
        int transactionId = 1;

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = senderWalletId, UserId = senderUserId, Name = "Test1", Balance = 1000},
            // new Wallet { Id = recipientWalletId, UserId = recipientUserId, Name = "Test2", Balance = 0}
        };

        List<User> users = new List<User>
        {
            new User { Username = "User1", Id = senderUserId },
            new User { Username = "User2", Id = recipientUserId },
        };

        List<Transaction> transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId, SenderUserId = senderUserId, Amount = 100, Description = "Test" },
        };
        
        FakeTransactionRepository transactionRepository = new FakeTransactionRepository(transactions);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        TransactionService service = new TransactionService(transactionRepository, walletRepository, userRepository, categoryRepository);

        TransactionCreateDto createDto = new TransactionCreateDto
        {
            SenderWalletId = senderWalletId,
            RecipientIdentifier = "User2",
            Amount = 100,
            Description = "Test"
        };
        
        await  Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.ExecuteTransactionAsync(senderUserId, createDto));
    }
    
    public class FakeTransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _transactions;

        public FakeTransactionRepository(List<Transaction> transactions)
        {
            _transactions = transactions;
        }
        
        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
        }

        public async Task<Transaction?> GetByIdWithDetailsAsync(int id)
        {
            return await Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
        }

        public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(int? walletId = null, int? userId = null, DateTime? startDate = null,
            DateTime? endDate = null, string? senderUsername = null, string? recipientUsername = null, string? direction = null,
            string? sortBy = null, bool isDescending = true, int pageNumber = 1, int pageSize = 10, int? categoryId = null)
        {
            IQueryable<Transaction> query = _transactions.AsQueryable();

            if (walletId.HasValue)
            {
                query = direction?.ToLower() switch
                {
                    "incoming" => query.Where(t => t.RecipientWalletId == walletId.Value),
                    "outgoing" => query.Where(t => t.SenderWalletId == walletId.Value),
                    _ => query.Where(t => t.SenderWalletId == walletId.Value || t.RecipientWalletId == walletId.Value)
                };
            }

            if (userId.HasValue)
            {
                query = direction?.ToLower() switch
                {
                    "incoming" => query.Where(t => t.RecipientUserId == userId.Value),
                    "outgoing" => query.Where(t => t.SenderUserId == userId.Value),
                    _ => query.Where(t => t.SenderUserId == userId.Value || t.RecipientUserId == userId.Value)
                };
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= endDate.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(senderUsername))
            {
                query = query.Where(t => t.SenderUser != null && t.SenderUser.Username == senderUsername);
            }

            if (!string.IsNullOrWhiteSpace(recipientUsername))
            {
                query = query.Where(t => t.RecipientUser != null && t.RecipientUser.Username == recipientUsername);
            }

            query = sortBy?.ToLower() switch
            {
                "amount" => isDescending ? query.OrderByDescending(t => t.Amount) : query.OrderBy(t => t.Amount),
                _ => isDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
            };

            IEnumerable<Transaction> result = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(result);
        }

        public Task AddAsync(Transaction transaction)
        {
            transaction.Id = _transactions.Count + 1;
            _transactions.Add(transaction);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SaveTransactionAndWalletsAsync(Wallet senderWallet, Wallet recipientWallet, Transaction transaction)
        {
            await AddAsync(transaction);
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

        public async Task<User?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task<User?> GetByIdWithDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Email == email));
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.PhoneNumber == phoneNumber));
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
            throw new NotImplementedException();
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

    public class FakeCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories;

        public FakeCategoryRepository(List<Category> categories)
        {
            _categories = categories;
        }
        
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId)
        {
            return await Task.FromResult(_categories.Where(c => c.UserId == userId));
        }

        public Task AddAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
