using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Security;

namespace VirtualWallet.Data.Context;

public static class DbInitializer
{
    public static void SeedTestData(VirtualWalletDbContext context, ICardDataProtector cardDataProtector)
    {
        // Check if test users already exist.
        if (context.Users.Any(u => u.Username == "john_doe"))
        {
            return; // DB has been seeded already
        }

        // Roles & Currencies are already seeded by EF Core OnModelCreating
        var userRole = context.Roles.FirstOrDefault(r => r.Name == "User") 
                       ?? new Role { Name = "User" };
        var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin") 
                        ?? new Role { Name = "Admin" };

        var eurCurrency = context.Currencies.FirstOrDefault(c => c.Code == "EUR") 
                          ?? new Currency { Code = "EUR", Name = "Euro", ExchangeRate = 1.0m };
        var usdCurrency = context.Currencies.FirstOrDefault(c => c.Code == "USD") 
                          ?? new Currency { Code = "USD", Name = "US Dollar", ExchangeRate = 1.08m };

        // 3. Create Test Users
        var admin = new User
        {
            Username = "admin",
            PasswordHash = "AQAAAAEAACcQAAAAEHASH_ADMIN_SECRET",
            Salt = "SALT_ADMIN",
            Email = "admin@virtualwallet.com",
            PhoneNumber = "0888111111",
            Role = adminRole,
            IsBlocked = false,
            IsEmailVerified = true,
            IdentityVerificationStatus = IdentityVerificationStatus.Approved,
            ReferralsUsed = 0,
            CreatedAt = DateTime.UtcNow
        };

        var john = new User
        {
            Username = "john_doe",
            PasswordHash = "AQAAAAEAACcQAAAAEHASH_JOHN_SECRET",
            Salt = "SALT_JOHN",
            Email = "john@virtualwallet.com",
            PhoneNumber = "0888222222",
            Role = userRole,
            IsBlocked = false,
            IsEmailVerified = true,
            IdentityVerificationStatus = IdentityVerificationStatus.Approved,
            ReferralsUsed = 0,
            CreatedAt = DateTime.UtcNow
        };

        var jane = new User
        {
            Username = "jane_smith",
            PasswordHash = "AQAAAAEAACcQAAAAEHASH_JANE_SECRET",
            Salt = "SALT_JANE",
            Email = "jane@virtualwallet.com",
            PhoneNumber = "0888333333",
            Role = userRole,
            IsBlocked = false,
            IsEmailVerified = true,
            IdentityVerificationStatus = IdentityVerificationStatus.Approved,
            ReferralsUsed = 0,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(admin, john, jane);
        context.SaveChanges();

        // 4. Create sample cards with protected PANs. CVVs are never persisted.
        var johnCard = new Card
        {
            ProtectedCardNumber = cardDataProtector.Protect("4111111111111111"),
            LastFour = "1111",
            ExpirationDate = DateTime.UtcNow.AddYears(3),
            CardHolder = "John Doe",
            IsPrimary = true,
            UserId = john.Id,
            CreatedAt = DateTime.UtcNow
        };

        var janeCard = new Card
        {
            ProtectedCardNumber = cardDataProtector.Protect("4222222222222222"),
            LastFour = "2222",
            ExpirationDate = DateTime.UtcNow.AddYears(2),
            CardHolder = "Jane Smith",
            IsPrimary = true,
            UserId = jane.Id,
            CreatedAt = DateTime.UtcNow
        };

        context.Cards.AddRange(johnCard, janeCard);
        context.SaveChanges();

        // 5. Create Wallets
        var johnWallet = new Wallet
        {
            Name = "Main Wallet",
            Balance = 1000.00m,
            IsDefault = true,
            Type = WalletType.Personal,
            UserId = john.Id,
            CurrencyId = eurCurrency.Id,
            IsOverdraftEnabled = true,
            OverdraftLimit = 200.00m,
            ConsecutiveNegativeMonths = 0,
            CreatedAt = DateTime.UtcNow
        };

        var johnSavingsWallet = new Wallet
        {
            Name = "USD Savings",
            Balance = 250.00m,
            IsDefault = false,
            Type = WalletType.Savings,
            UserId = john.Id,
            CurrencyId = usdCurrency.Id,
            SavingsAmount = 250.00m,
            SavingsInterestRate = 0.04m,
            SavingsMaturityDate = DateTime.UtcNow.AddMonths(6),
            CreatedAt = DateTime.UtcNow
        };

        var janeWallet = new Wallet
        {
            Name = "Main Wallet",
            Balance = 500.00m,
            IsDefault = true,
            Type = WalletType.Personal,
            UserId = jane.Id,
            CurrencyId = eurCurrency.Id,
            IsOverdraftEnabled = false,
            OverdraftLimit = 0.00m,
            ConsecutiveNegativeMonths = 0,
            CreatedAt = DateTime.UtcNow
        };

        context.Wallets.AddRange(johnWallet, johnSavingsWallet, janeWallet);
        context.SaveChanges();

        // 6. Create Categories
        var groceriesCat = new Category { Name = "Groceries", UserId = john.Id };
        var utilitiesCat = new Category { Name = "Utilities", UserId = john.Id };
        var restaurantsCat = new Category { Name = "Restaurants", UserId = jane.Id };

        context.Categories.AddRange(groceriesCat, utilitiesCat, restaurantsCat);
        context.SaveChanges();

        // 7. Create Contacts
        var contact = new Contact { UserId = john.Id, ContactUserId = jane.Id, CreatedAt = DateTime.UtcNow };
        context.Contacts.Add(contact);
        context.SaveChanges();

        // 8. Create Sample Transfers & Transactions
        var topup = new Transfer
        {
            Amount = 200.00m,
            CardId = johnCard.Id,
            WalletId = johnWallet.Id,
            Status = TransferStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        var transaction = new Transaction
        {
            Amount = 45.50m,
            Description = "Dinner & drinks split",
            SenderUserId = john.Id,
            SenderWalletId = johnWallet.Id,
            RecipientUserId = jane.Id,
            RecipientWalletId = janeWallet.Id,
            Status = TransactionStatus.Confirmed,
            CategoryId = groceriesCat.Id,
            IsVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Transfers.Add(topup);
        context.Transactions.Add(transaction);
        context.SaveChanges();
    }
}
