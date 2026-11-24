using VirtualWallet.Business.DTOs.Users;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Security;
using VirtualWallet.Business.Services.Implementations;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task RegisterAsync_ReturnsResponse()
    {
        int userId = 1;

        List<User> users = new();
        List<Wallet> wallets = new();
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(wallets);
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "Password1!",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        var result = await service.RegisterAsync(registerDto);
        
        var savedUser = Assert.Single(users);
        
        Assert.NotEqual(registerDto.Password, savedUser.PasswordHash);
        Assert.True(PasswordHasher.VerifyPassword(
            registerDto.Password,
            savedUser.PasswordHash,
            savedUser.Salt));
        
        var wallet = Assert.Single(wallets);
        Assert.Equal(result.Id, wallet.UserId);
        Assert.Equal("Main Wallet", wallet.Name);
        Assert.Equal(0m, wallet.Balance);
        Assert.True(wallet.IsDefault);
        
        Assert.Equal(userId, result.Id);
        Assert.Equal(registerDto.Email, result.Email);
        Assert.Equal(registerDto.PhoneNumber, result.PhoneNumber);
        Assert.Equal(registerDto.Username, result.Username);
    }
    
    [Fact]
    public async Task RegisterAsync_ThrowsInvalidUserInputException_WhenPasswordIsEmpty()
    {
        FakeUserRepository userRepository = new FakeUserRepository(new List<User>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        await Assert.ThrowsAsync<InvalidUserInputException> (() 
            => service.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ThrowsInvalidUserInputException_WhenPasswordIsLessCharacters()
    {
        FakeUserRepository userRepository = new FakeUserRepository(new List<User>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "passwor",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        await Assert.ThrowsAsync<InvalidUserInputException> (() 
            => service.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ThrowsInvalidUserInputException_WhenPasswordDoesntContainCapLetter()
    {
        FakeUserRepository userRepository = new FakeUserRepository(new List<User>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "password!",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        await Assert.ThrowsAsync<InvalidUserInputException> (() 
            => service.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ThrowsInvalidUserInputException_WhenPasswordDoesntContainDigit()
    {
        FakeUserRepository userRepository = new FakeUserRepository(new List<User>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "Password!",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        await Assert.ThrowsAsync<InvalidUserInputException> (() 
            => service.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ThrowsInvalidUserInputException_WhenPasswordDoesntContainSpecialChar()
    {
        FakeUserRepository userRepository = new FakeUserRepository(new List<User>());
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "Password1",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        await Assert.ThrowsAsync<InvalidUserInputException> (() 
            => service.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ThrowsDuplicateEntityException_WhenUsernameIsTaken()
    {
        int userId = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId, Username = "User" },
        };

        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "Password1!",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        await Assert.ThrowsAsync<DuplicateEntityException> (() 
            => service.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ThrowsDuplicateEntityException_WhenEmailIsTaken()
    {
        int userId = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId, Email =  "Email" },
        };

        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "Password1!",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        await Assert.ThrowsAsync<DuplicateEntityException> (() 
            => service.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ThrowsDuplicateEntityException_WhenPhoneNumIsTaken()
    {
        int userId = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId, PhoneNumber =  "PhoneNumber" },
        };

        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserRegisterDto registerDto = new UserRegisterDto
        {
            Username = "User",
            Password = "Password1!",
            Email = "Email",
            PhoneNumber = "PhoneNumber"
        };
        
        await Assert.ThrowsAsync<DuplicateEntityException> (() 
            => service.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsResponse()
    {
        int userId = 1;

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = "User",
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt, PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserLoginDto loginDto = new UserLoginDto
        {
            Username = "User",
            Password = "Password1!",
        };
        
        var result = await service.AuthenticateAsync(loginDto);
        
        Assert.Equal(userId, result.Id);
        Assert.Equal("User", result.Username);
        Assert.Equal("Email", result.Email);
        Assert.Equal("PhoneNumber", result.PhoneNumber);
    }
    
    [Fact]
    public async Task AuthenticateAsync_ThrowsInvalidUserInputException_WhenPasswordIsWrong()
    {
        int userId = 1;

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = "User",
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt, PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserLoginDto loginDto = new UserLoginDto
        {
            Username = "User",
            Password = "Password1",
        };
        
        await  Assert.ThrowsAsync<InvalidUserInputException> (()
            => service.AuthenticateAsync(loginDto));
    }
    
    [Fact]
    public async Task AuthenticateAsync_ThrowsInvalidUserInputException_WhenUsernameIsWrong()
    {
        int userId = 1;

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = "User",
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt, PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserLoginDto loginDto = new UserLoginDto
        {
            Username = "User1",
            Password = "Password1!",
        };
        
        await  Assert.ThrowsAsync<InvalidUserInputException> (()
            => service.AuthenticateAsync(loginDto));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsResponse()
    {
        int userId = 1;

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = "User",
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        var result = await service.GetByIdAsync(userId);
        
        Assert.Equal(userId, result.Id);
        Assert.Equal("User", result.Username);
        Assert.Equal("PhoneNumber", result.PhoneNumber);
        Assert.Equal("Email", result.Email);
    }
    
    [Fact]
    public async Task GetByIdAsync_ThrowsEntityNotFoundException_WhenUserDoesNotExist()
    {
        int userId = 1;
        int missingId = 2;

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = "User",
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        await  Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.GetByIdAsync(missingId));
    }
    
    [Fact]
    public async Task GetByUsernameAsync_ReturnsResponse()
    {
        int userId = 1;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        var result = await service.GetByUsernameAsync(username);
        
        Assert.Equal(userId, result.Id);
        Assert.Equal("User", result.Username);
        Assert.Equal("PhoneNumber", result.PhoneNumber);
        Assert.Equal("Email", result.Email);
    }
    
    [Fact]
    public async Task GetByUsernameAsync_ThrowsEntityNotFoundException_WhenUserDoesNotExist()
    {
        int userId = 1;
        
        string username = "User1";
        string missingUsername = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        await  Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.GetByUsernameAsync(missingUsername));
    }
    
    [Fact]
    public async Task FindRecipientAsync_ReturnsResponse_WhenMatchingUsername()
    {
        int userId = 1;
        int user2Id = 2;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        string password2 = "Password2!";
        string salt2 = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = user2Id,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
                Salt =  salt2,
                PasswordHash = PasswordHasher.HashPassword(password2, salt2)
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        var result = await service.FindRecipientAsync(username);
        
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("User", result.Username);
        Assert.Equal("PhoneNumber", result.PhoneNumber);
        Assert.Equal("Email", result.Email);
    }
    
    [Fact]
    public async Task FindRecipientAsync_ReturnsResponse_WhenIdentifierIsNull()
    {
        int userId = 1;
        int user2Id = 2;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        string password2 = "Password2!";
        string salt2 = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = user2Id,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
                Salt =  salt2,
                PasswordHash = PasswordHasher.HashPassword(password2, salt2)
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        string? identifier = null;

        var result = await service.FindRecipientAsync(identifier);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task FindRecipientAsync_ReturnsResponse_WhenMatchingEmail()
    {
        int userId = 1;
        int user2Id = 2;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        string password2 = "Password2!";
        string salt2 = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = user2Id,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
                Salt =  salt2,
                PasswordHash = PasswordHasher.HashPassword(password2, salt2)
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        string identifier = "Email";

        var result = await service.FindRecipientAsync(identifier);
        
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("User", result.Username);
        Assert.Equal("PhoneNumber", result.PhoneNumber);
        Assert.Equal("Email", result.Email);
    }
    
    [Fact]
    public async Task FindRecipientAsync_ReturnsResponse_WhenMatchingPhoneNumber()
    {
        int userId = 1;
        int user2Id = 2;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        string password2 = "Password2!";
        string salt2 = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = user2Id,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
                Salt =  salt2,
                PasswordHash = PasswordHasher.HashPassword(password2, salt2)
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        string identifier = "PhoneNumber";

        var result = await service.FindRecipientAsync(identifier);
        
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("User", result.Username);
        Assert.Equal("PhoneNumber", result.PhoneNumber);
        Assert.Equal("Email", result.Email);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        int userId = 1;
        int user2Id = 2;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        string password2 = "Password2!";
        string salt2 = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = user2Id,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
                Salt =  salt2,
                PasswordHash = PasswordHasher.HashPassword(password2, salt2)
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        var result = await service.GetAllAsync();
        
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public async Task UpdateProfileAsync_UpdatesProfile()
    {
        int userId = 1;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserProfileUpdateDto updateDto = new UserProfileUpdateDto
        {
            Email = "EmailUpdated",
            PhoneNumber = "PhoneNumberUpdated",
        };

        var result = await service.UpdateProfileAsync(userId, updateDto);
        
        Assert.NotNull(result);
        Assert.Equal("EmailUpdated", result.Email);
        Assert.Equal("PhoneNumberUpdated", result.PhoneNumber);
    }
    
    [Fact]
    public async Task UpdateProfileAsync_ThrowsEntityNotFoundException_WhenUserDoesntExist()
    {
        int userId = 1;
        int missingUserId = 2;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserProfileUpdateDto updateDto = new UserProfileUpdateDto
        {
            Email = "EmailUpdated",
            PhoneNumber = "PhoneNumberUpdated",
        };

        await Assert.ThrowsAsync<EntityNotFoundException>(() 
            => service.UpdateProfileAsync(missingUserId, updateDto));
    }
    
    [Fact]
    public async Task UpdateProfileAsync_ThrowsDuplicateEntityException_WhenEmailIsUsed()
    {
        int userId = 1;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = 2,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserProfileUpdateDto updateDto = new UserProfileUpdateDto
        {
            Email = "Email2",
            PhoneNumber = "PhoneNumberUpdated",
        };

        await  Assert.ThrowsAsync<DuplicateEntityException>(()
            => service.UpdateProfileAsync(userId, updateDto));
    }
    
    [Fact]
    public async Task UpdateProfileAsync_ThrowsDuplicateEntityException_WhenPhoneNumberIsUsed()
    {
        int userId = 1;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = 2,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);

        UserProfileUpdateDto updateDto = new UserProfileUpdateDto
        {
            Email = "EmailUpdated",
            PhoneNumber = "PhoneNumber2",
        };

        await  Assert.ThrowsAsync<DuplicateEntityException>(()
            => service.UpdateProfileAsync(userId, updateDto));
    }

    [Fact]
    public async Task BlockUserAsync_BlocksUser()
    {
        int userId = 1;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = 2,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        await service.BlockUserAsync(userId);
        
        Assert.True(users.First().IsBlocked);
    }
    
    [Fact]
    public async Task BlockUserAsync_ThrowsEntityNotFoundException_WhenUserDoesntExist()
    {
        int userId = 1;
        int missingUserId = 2;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.BlockUserAsync(missingUserId));
    }

    [Fact]
    public async Task UnblockUserAsync_UnblocksUser()
    {
        int userId = 1;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
            new User
            {
                Id = 2,
                Username = "User2",
                PhoneNumber = "PhoneNumber2",
                Email =  "Email2",
            }
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        await service.UnblockUserAsync(userId);
        
        Assert.False(users.First().IsBlocked);
    }
    
    [Fact]
    public async Task UnblockUserAsync_ThrowsEntityNotFoundException_WhenUserDoesntExist()
    {
        int userId = 1;
        int missingUserId = 2;
        
        string username = "User";

        string password = "Password1!";
        string salt = PasswordHasher.GenerateSalt();
        
        List<User> users = new List<User>
        {
            new User
            {
                Id = userId,
                Username = username,
                PhoneNumber = "PhoneNumber",
                Email =  "Email",
                Salt =  salt,
                PasswordHash =  PasswordHasher.HashPassword(password, salt)
            },
        };
        
        FakeUserRepository userRepository = new FakeUserRepository(users);
        FakeWalletRepository walletRepository = new FakeWalletRepository(new List<Wallet>());
        
        UserService service = new UserService(userRepository, walletRepository);
        
        await  Assert.ThrowsAsync<EntityNotFoundException>(()
            => service.UnblockUserAsync(missingUserId));
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
            return Task.FromResult(
                _users.AsEnumerable());
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
            return Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Email == email));
        }

        public Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.PhoneNumber == phoneNumber));
        }

        public Task<bool> ExistsByUsernameAsync(string username)
        {
            return Task.FromResult(_users.Any(u => u.Username == username));
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            return Task.FromResult(_users.Any(u => u.Email == email));
        }

        public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
        {
            return Task.FromResult(_users.Any(u => u.PhoneNumber == phoneNumber));
        }

        public Task AddAsync(User user)
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            int index = _users.FindIndex(u => u.Id == user.Id);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"User with id {user.Id} was not found.");
            }
        
            _users[index] = user;
            return Task.CompletedTask;
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
}