using VirtualWallet.Business.DTOs.Users;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Security;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Business.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;

    public UserService(IUserRepository userRepository, IWalletRepository walletRepository)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
    }

    public async Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto)
    {
        var passwordValidationError = PasswordValidator.Validate(registerDto.Password);
        if (passwordValidationError != null)
        {
            throw new InvalidUserInputException(passwordValidationError);
        }

        if (await _userRepository.ExistsByUsernameAsync(registerDto.Username))
        {
            throw new DuplicateEntityException($"Username '{registerDto.Username}' is already taken.");
        }

        if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
        {
            throw new DuplicateEntityException($"Email '{registerDto.Email}' is already registered.");
        }

        if (await _userRepository.ExistsByPhoneNumberAsync(registerDto.PhoneNumber))
        {
            throw new DuplicateEntityException($"Phone number '{registerDto.PhoneNumber}' is already in use.");
        }

        var salt = PasswordHasher.GenerateSalt();
        var passwordHash = PasswordHasher.HashPassword(registerDto.Password, salt);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            PasswordHash = passwordHash,
            Salt = salt,
            RoleId = 1, // Standard User role
            IsBlocked = false,
            IsEmailVerified = false,
            IdentityVerificationStatus = IdentityVerificationStatus.NotSubmitted,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        // Create default personal wallet in EUR (CurrencyId = 1)
        var defaultWallet = new Wallet
        {
            Name = "Main Wallet",
            Balance = 0m,
            IsDefault = true,
            Type = WalletType.Personal,
            UserId = user.Id,
            CurrencyId = 1,
            IsOverdraftEnabled = false,
            OverdraftLimit = 0m,
            CreatedAt = DateTime.UtcNow
        };

        await _walletRepository.AddAsync(defaultWallet);

        return MapToDto(user);
    }

    public async Task<UserResponseDto> AuthenticateAsync(UserLoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash, user.Salt))
        {
            throw new InvalidUserInputException("Invalid username or password.");
        }

        return MapToDto(user);
    }

    public async Task<UserResponseDto> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id)
                   ?? throw new EntityNotFoundException($"User with ID {id} was not found.");

        return MapToDto(user);
    }

    public async Task<UserResponseDto> GetByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username)
                   ?? throw new EntityNotFoundException($"User '{username}' was not found.");

        return MapToDto(user);
    }

    public async Task<UserResponseDto?> FindRecipientAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return null;

        var trimmed = identifier.Trim();

        var user = await _userRepository.GetByUsernameAsync(trimmed)
                   ?? await _userRepository.GetByEmailAsync(trimmed)
                   ?? await _userRepository.GetByPhoneNumberAsync(trimmed);

        return user != null ? MapToDto(user) : null;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserResponseDto> UpdateProfileAsync(int userId, UserProfileUpdateDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new EntityNotFoundException($"User with ID {userId} was not found.");

        if (!user.Email.Equals(updateDto.Email, StringComparison.OrdinalIgnoreCase)
            && await _userRepository.ExistsByEmailAsync(updateDto.Email))
        {
            throw new DuplicateEntityException($"Email '{updateDto.Email}' is already registered.");
        }

        if (user.PhoneNumber != updateDto.PhoneNumber
            && await _userRepository.ExistsByPhoneNumberAsync(updateDto.PhoneNumber))
        {
            throw new DuplicateEntityException($"Phone number '{updateDto.PhoneNumber}' is already in use.");
        }

        user.Email = updateDto.Email;
        user.PhoneNumber = updateDto.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(updateDto.PhotoUrl))
        {
            user.PhotoUrl = updateDto.PhotoUrl;
        }
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return MapToDto(user);
    }

    public async Task BlockUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new EntityNotFoundException($"User with ID {userId} was not found.");

        user.IsBlocked = true;
        await _userRepository.UpdateAsync(user);
    }

    public async Task UnblockUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new EntityNotFoundException($"User with ID {userId} was not found.");

        user.IsBlocked = false;
        await _userRepository.UpdateAsync(user);
    }

    private static UserResponseDto MapToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            PhotoUrl = user.PhotoUrl,
            Role = user.Role?.Name ?? "User",
            IsBlocked = user.IsBlocked,
            IsEmailVerified = user.IsEmailVerified,
            IdentityVerificationStatus = user.IdentityVerificationStatus.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
