using VirtualWallet.Business.DTOs.Wallets;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Implementations;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Tests;

public class WalletServiceTests
{
    [Fact]
    public async Task CreateWalletAsync_ReturnsResponse()
    {
        int userId = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" },
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        WalletCreateDto createDto = new WalletCreateDto
        {
            Name =  "Test",
            CurrencyId = 1
        };

        var result = await service.CreateWalletAsync(userId, createDto);
        
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.Name);
        Assert.True(result.IsDefault);
        Assert.Equal(0, result.Balance);
    }
    
    [Fact]
    public async Task CreateWalletAsync_ThrowsEntityNotFoundException_WhenUserDoesntExist()
    {
        int userId = 1;
        int missingUserId = 2;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" },
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        WalletCreateDto createDto = new WalletCreateDto
        {
            Name =  "Test",
            CurrencyId = 1
        };

        await Assert.ThrowsAsync<EntityNotFoundException>(async () 
            => await service.CreateWalletAsync(missingUserId, createDto));
    }
    
    [Fact]
    public async Task GetUserWalletsAsync_ReturnsUserWallets()
    {
        int user1Id = 1;
        int user2Id = 2;

        List<User> users = new List<User>
        {
            new User { Id = user1Id, Username = "user1" },
            new  User { Id = user2Id, Username = "user2" },
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = 1, CurrencyId = 1 , UserId = user1Id , Name = "User1Wallet1", Balance = 200},
            new Wallet { Id = 2, CurrencyId = 2 , UserId = user1Id, Name = "User1Wallet2", Balance = 100},
            new Wallet { Id = 3, CurrencyId = 2 , UserId = user2Id , Name =  "User2Wallet1"},
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        var result = await service.GetUserWalletsAsync(user1Id);
        
        Assert.Equal(2, result.Count());
        Assert.Equal(user1Id, result.First().UserId);
        Assert.Equal(user1Id, result.Last().UserId);
        Assert.Equal(200, result.First().Balance);
        Assert.Equal(100, result.Last().Balance);
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsWallet()
    {
        int userId = 1;
        int walletId = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = walletId, CurrencyId = 1 , UserId = userId , Name = "Wallet1", Balance = 200}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        var result = await service.GetByIdAsync(walletId, userId);
        
        Assert.Equal(walletId, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(200, result.Balance);
        Assert.Equal("Wallet1", result.Name);
    }
    
    [Fact]
    public async Task GetByIdAsync_ThrowsEntityNotFoundException_WhenWalletDoesNotExist()
    {
        int userId = 1;
        int walletId = 1;
        
        int missingWalletId = 2;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = walletId, CurrencyId = 1 , UserId = userId , Name = "Wallet1", Balance = 200}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        await Assert.ThrowsAsync<EntityNotFoundException>(async ()
            => await service.GetByIdAsync(missingWalletId, userId));
    }
    
    [Fact]
    public async Task GetByIdAsync_ThrowsUnauthorizedOperationException_WhenWalletDoesntBelongToUser()
    {
        int userId = 1;
        int walletId = 1;
        
        int missingUserId = 2;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" },
        };

        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = walletId, CurrencyId = 1, UserId = missingUserId, Name = "Wallet1", Balance = 200 }
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        await Assert.ThrowsAsync<UnauthorizedOperationException>(async ()
            => await service.GetByIdAsync(walletId, userId));
    }

    [Fact]
    public async Task SetDefaultWalletAsync_SetsDefaultWallet()
    {
        int userId = 1;
        int wallet1Id = 1;
        int wallet2Id = 2;
        
        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = userId , Name = "Wallet1", Balance = 200, IsDefault = false},
            new Wallet { Id = wallet2Id, CurrencyId = 2 , UserId = userId , Name = "Wallet2", Balance = 200, IsDefault = true}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await service.SetDefaultWalletAsync(wallet1Id, userId);
        
        var savedWallets = await service.GetUserWalletsAsync(userId);
        
        Assert.True(savedWallets.First().IsDefault);
        Assert.False(savedWallets.Last().IsDefault);
    }
    
    [Fact]
    public async Task SetDefaultWalletAsync_ThrowsEntityNotFoundException_WhenWalletDoesNotExist()
    {
        int userId = 1;
        
        int wallet1Id = 1;
        int wallet2Id = 2;
        int missingWalletId = 3;
        
        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = userId , Name = "Wallet1", Balance = 200, IsDefault = false},
            new Wallet { Id = wallet2Id, CurrencyId = 2 , UserId = userId , Name = "Wallet2", Balance = 200, IsDefault = true}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(async 
            () => await service.SetDefaultWalletAsync(missingWalletId, userId));
    }
    
    [Fact]
    public async Task SetDefaultWalletAsync_ThrowsUnauthorizedOperationException_WhenWalletDoesntBelongToUser()
    {
        int user1Id = 1;
        int user2Id = 2;
        
        int wallet1Id = 1;
        int wallet2Id = 2;
        
        List<User> users = new List<User>
        {
            new User { Id = user1Id, Username = "user1" },
            new User { Id = user2Id, Username = "user2" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = user1Id , Name = "Wallet1", Balance = 200, IsDefault = false},
            new Wallet { Id = wallet2Id, CurrencyId = 2 , UserId = user1Id , Name = "Wallet2", Balance = 200, IsDefault = true}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>(async 
            () => await service.SetDefaultWalletAsync(wallet1Id, user2Id));
    }
    
    [Fact]
    public async Task DeleteWalletAsync_DeletesWallet()
    {
        int user1Id = 1;
        
        int wallet1Id = 1;
        int wallet2Id = 2;
        
        List<User> users = new List<User>
        {
            new User { Id = user1Id, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = user1Id , Name = "Wallet1", Balance = 0, IsDefault = false},
            new Wallet { Id = wallet2Id, CurrencyId = 2 , UserId = user1Id , Name = "Wallet2", Balance = 200, IsDefault = true}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await service.DeleteWalletAsync(wallet1Id, user1Id);
        
        Assert.Single(wallets);
    }
    
    [Fact]
    public async Task DeleteWalletAsync_ThrowsEntityNotFoundException_WhenWalletDoesNotExist()
    {
        int user1Id = 1;
        
        int wallet1Id = 1;
        int wallet2Id = 2;
        int missingWalletId = 3;
        
        List<User> users = new List<User>
        {
            new User { Id = user1Id, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = user1Id , Name = "Wallet1", Balance = 0, IsDefault = false},
            new Wallet { Id = wallet2Id, CurrencyId = 2 , UserId = user1Id , Name = "Wallet2", Balance = 200, IsDefault = true}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(async
            () => await service.DeleteWalletAsync(missingWalletId, user1Id));
    }
    
    [Fact]
    public async Task DeleteWalletAsync_ThrowsUnauthorizedOperationException_WhenWalletDoesntBelongToUser()
    {
        int user1Id = 1;
        int user2Id = 2;
        
        int wallet1Id = 1;
        int wallet2Id = 2;
        
        List<User> users = new List<User>
        {
            new User { Id = user1Id, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = user2Id , Name = "Wallet1", Balance = 0, IsDefault = false},
            new Wallet { Id = wallet2Id, CurrencyId = 2 , UserId = user2Id , Name = "Wallet2", Balance = 200, IsDefault = true}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>(async
            () => await service.DeleteWalletAsync(wallet1Id, user1Id));
    }
    
    [Fact]
    public async Task DeleteWalletAsync_ThrowsInvalidUserInputException_WhenWalletHasPositiveBalance()
    {
        int user1Id = 1;
        
        int wallet1Id = 1;
        int wallet2Id = 2;
        
        List<User> users = new List<User>
        {
            new User { Id = user1Id, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = user1Id , Name = "Wallet1", Balance = 200, IsDefault = false},
            new Wallet { Id = wallet2Id, CurrencyId = 2 , UserId = user1Id , Name = "Wallet2", Balance = 200, IsDefault = true}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await Assert.ThrowsAsync<InvalidUserInputException>(async
            () => await service.DeleteWalletAsync(wallet1Id, user1Id));
    }
    
    [Fact]
    public async Task DeleteWalletAsync_ThrowsInvalidUserInputException_WhenWalletIsOnlyOneUserOwns()
    {
        int user1Id = 1;

        int wallet1Id = 1;
        
        List<User> users = new List<User>
        {
            new User { Id = user1Id, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = user1Id , Name = "Wallet1", Balance = 0, IsDefault = false},
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await Assert.ThrowsAsync<InvalidUserInputException>(async
            () => await service.DeleteWalletAsync(wallet1Id, user1Id));
    }

    [Fact]
    public async Task DeleteWalletAsync_SetsRemainingWalletAsDefault()
    {
        int user1Id = 1;

        int wallet1Id = 1;
        int wallet2Id = 2;
        
        List<User> users = new List<User>
        {
            new User { Id = user1Id, Username = "user1" },
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = user1Id , Name = "Wallet1", Balance = 0, IsDefault = true},
            new Wallet { Id = wallet2Id, CurrencyId = 1 , UserId = user1Id , Name = "Wallet2", Balance = 200, IsDefault = false}
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);
        
        await service.DeleteWalletAsync(wallet1Id, user1Id);
        
        Assert.Single(wallets);
        Assert.True(wallets[0].IsDefault);
    }

    [Fact]
    public async Task CreateSavingsWalletAsync_CreatesSavingsWallet()
    {
        int userId = 1;
        int wallet1Id = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" }
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = userId , Name = "Wallet1", Balance = 200, IsDefault = false},
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        SavingsWalletCreateDto createDto = new SavingsWalletCreateDto
        {
            Name = "SavingWallet1",
            SourceWalletId = wallet1Id,
            Amount = 100,
            DurationMonths = 12
        };
        
        var result = await service.CreateSavingsWalletAsync(userId, createDto);
        var savedWallet = await service.GetByIdAsync(wallet1Id,userId);
        
        Assert.Equal("Savings",result.Type);
        Assert.Equal(100, result.Balance);
        Assert.Equal("SavingWallet1", result.Name);
        Assert.Equal(100, savedWallet.Balance);
        Assert.Equal(2, result.Id);
    }
    
    [Fact]
    public async Task CreateSavingsWalletAsync_ThrowsEntityNotFoundException_WhenSourceWalletDoesNotExist()
    {
        int userId = 1;
        int wallet1Id = 1;
        int missingWalletId = 2;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" }
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = userId , Name = "Wallet1", Balance = 200, IsDefault = false},
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        SavingsWalletCreateDto createDto = new SavingsWalletCreateDto
        {
            Name = "SavingWallet1",
            SourceWalletId = missingWalletId,
            Amount = 100,
            DurationMonths = 12
        };
        
        await Assert.ThrowsAsync<EntityNotFoundException>(async
            () => await service.CreateSavingsWalletAsync(userId, createDto));
    }
    
    [Fact]
    public async Task CreateSavingsWalletAsync_ThrowsInsufficientFundsException_WhenSourceWalletDoesntHaveEnoughFunds()
    {
        int userId = 1;
        int wallet1Id = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" }
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = userId , Name = "Wallet1", Balance = 50, IsDefault = false},
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        SavingsWalletCreateDto createDto = new SavingsWalletCreateDto
        {
            Name = "SavingWallet1",
            SourceWalletId = wallet1Id,
            Amount = 100,
            DurationMonths = 12
        };
        
        await Assert.ThrowsAsync<InsufficientFundsException>(async
            () => await service.CreateSavingsWalletAsync(userId, createDto));
    }
    
    [Fact]
    public async Task CreateSavingsWalletAsync_ThrowsUnauthorizedOperationException_WhenSourceWalletDoesntBelongToUser()
    {
        int userId = 1;
        int wallet1Id = 1;
        int missingUserId = 2;
        
        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "user1" }
        };
        
        List<Wallet> wallets = new List<Wallet>
        {
            new Wallet { Id = wallet1Id, CurrencyId = 1 , UserId = missingUserId , Name = "Wallet1", Balance = 200, IsDefault = false},
        };
        
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        
        WalletService service = new WalletService(walletRepository, userRepository);

        SavingsWalletCreateDto createDto = new SavingsWalletCreateDto
        {
            Name = "SavingWallet1",
            SourceWalletId = wallet1Id,
            Amount = 100,
            DurationMonths = 12
        };
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>(async
            () => await service.CreateSavingsWalletAsync(userId, createDto));
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

        public Task<IEnumerable<Wallet>> GetByUserIdAsync(int userId)
        {
            IEnumerable<Wallet> result = _wallets.Where(w => w.UserId == userId).ToList();
            return Task.FromResult(result);
        }

        public Task AddAsync(Wallet wallet)
        {
            wallet.Id = _wallets.Count + 1;
            _wallets.Add(wallet);
            return Task.CompletedTask;
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
            _wallets.Remove(wallet);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateSavingsWalletWithTransfersAsync(Wallet sourceWallet, Wallet savingsWallet)
        {
            savingsWallet.Type = WalletType.Savings;
            savingsWallet.Id = _wallets.Count + 1;
            _wallets.Add(savingsWallet);
            
            return Task.CompletedTask;
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
}