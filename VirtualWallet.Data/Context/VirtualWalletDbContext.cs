using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Models;

namespace VirtualWallet.Data.Context;

public class VirtualWalletDbContext : DbContext
{
    public VirtualWalletDbContext(DbContextOptions<VirtualWalletDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletMember> WalletMembers => Set<WalletMember>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<RecurringTransaction> RecurringTransactions => Set<RecurringTransaction>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<OverdraftSetting> OverdraftSettings => Set<OverdraftSetting>();
    public DbSet<VerificationCode> VerificationCodes => Set<VerificationCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureCard(modelBuilder);
        ConfigureWallet(modelBuilder);
        ConfigureWalletMember(modelBuilder);
        ConfigureCurrency(modelBuilder);
        ConfigureTransaction(modelBuilder);
        ConfigureTransfer(modelBuilder);
        ConfigureCategory(modelBuilder);
        ConfigureRecurringTransaction(modelBuilder);
        ConfigureContact(modelBuilder);
        ConfigureReferral(modelBuilder);
        ConfigureOverdraftSetting(modelBuilder);
        ConfigureVerificationCode(modelBuilder);

        SeedData(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Salt)
                .IsRequired();

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(254);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasIndex(u => u.PhoneNumber)
                .IsUnique();

            entity.Property(u => u.PhotoUrl)
                .HasMaxLength(500);

            entity.Property(u => u.IdCardPhotoUrl)
                .HasMaxLength(500);

            entity.Property(u => u.SelfiePhotoUrl)
                .HasMaxLength(500);

            entity.Property(u => u.IdentityVerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(r => r.Name)
                .IsUnique();
        });
    }

    private static void ConfigureCard(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.ProtectedCardNumber)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(c => c.LastFour)
                .IsRequired()
                .HasMaxLength(4)
                .IsFixedLength();

            entity.Property(c => c.CardHolder)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(c => c.User)
                .WithMany(u => u.Cards)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureWallet(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(w => w.Id);

            entity.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(w => w.Balance)
                .HasPrecision(18, 2);

            entity.Property(w => w.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(w => w.OverdraftLimit)
                .HasPrecision(18, 2);

            // Savings fields
            entity.Property(w => w.SavingsAmount)
                .HasPrecision(18, 2);

            entity.Property(w => w.SavingsInterestRate)
                .HasPrecision(8, 4);

            entity.Property(w => w.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(w => w.User)
                .WithMany(u => u.Wallets)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(w => w.Currency)
                .WithMany(c => c.Wallets)
                .HasForeignKey(w => w.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing FK: Savings wallet => source wallet
            entity.HasOne(w => w.SourceWallet)
                .WithMany()
                .HasForeignKey(w => w.SourceWalletId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureWalletMember(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WalletMember>(entity =>
        {
            entity.HasKey(wm => wm.Id);

            entity.HasIndex(wm => new { wm.WalletId, wm.UserId })
                .IsUnique();

            entity.Property(wm => wm.AddedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(wm => wm.Wallet)
                .WithMany(w => w.Members)
                .HasForeignKey(wm => wm.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(wm => wm.User)
                .WithMany(u => u.WalletMemberships)
                .HasForeignKey(wm => wm.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureCurrency(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(3)
                .IsFixedLength();

            entity.HasIndex(c => c.Code)
                .IsUnique();

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(c => c.ExchangeRate)
                .HasPrecision(18, 6);
        });
    }

    private static void ConfigureTransaction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Amount)
                .HasPrecision(18, 2);

            entity.Property(t => t.Description)
                .HasMaxLength(500);

            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.VerificationCode)
                .HasMaxLength(10);

            entity.Property(t => t.ExchangeRate)
                .HasPrecision(18, 6);

            entity.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Sender User
            entity.HasOne(t => t.SenderUser)
                .WithMany(u => u.SentTransactions)
                .HasForeignKey(t => t.SenderUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Recipient User
            entity.HasOne(t => t.RecipientUser)
                .WithMany(u => u.ReceivedTransactions)
                .HasForeignKey(t => t.RecipientUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Sender Wallet
            entity.HasOne(t => t.SenderWallet)
                .WithMany(w => w.OutgoingTransactions)
                .HasForeignKey(t => t.SenderWalletId)
                .OnDelete(DeleteBehavior.NoAction);

            // Recipient Wallet
            entity.HasOne(t => t.RecipientWallet)
                .WithMany(w => w.IncomingTransactions)
                .HasForeignKey(t => t.RecipientWalletId)
                .OnDelete(DeleteBehavior.NoAction);

            // Category
            entity.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Recurring Transaction
            entity.HasOne(t => t.RecurringTransaction)
                .WithMany(rt => rt.Transactions)
                .HasForeignKey(t => t.RecurringTransactionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for filtering/sorting
            entity.HasIndex(t => t.CreatedAt);
            entity.HasIndex(t => t.Status);
        });
    }

    private static void ConfigureTransfer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Amount)
                .HasPrecision(18, 2);

            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(t => t.Card)
                .WithMany(c => c.Transfers)
                .HasForeignKey(t => t.CardId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(t => t.Wallet)
                .WithMany(w => w.Transfers)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(t => t.CreatedAt);
        });
    }

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(c => new { c.UserId, c.Name })
                .IsUnique();

            entity.HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureRecurringTransaction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecurringTransaction>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.Amount)
                .HasPrecision(18, 2);

            entity.Property(rt => rt.Description)
                .HasMaxLength(500);

            entity.Property(rt => rt.Interval)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(rt => rt.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(rt => rt.SenderWallet)
                .WithMany()
                .HasForeignKey(rt => rt.SenderWalletId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(rt => rt.RecipientWallet)
                .WithMany()
                .HasForeignKey(rt => rt.RecipientWalletId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(rt => rt.Category)
                .WithMany()
                .HasForeignKey(rt => rt.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(rt => rt.NextExecutionDate);
            entity.HasIndex(rt => rt.IsActive);
        });
    }

    private static void ConfigureContact(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasIndex(c => new { c.UserId, c.ContactUserId })
                .IsUnique();

            entity.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(c => c.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.ContactUser)
                .WithMany(u => u.ContactOf)
                .HasForeignKey(c => c.ContactUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureReferral(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Referral>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.InviteeEmail)
                .IsRequired()
                .HasMaxLength(254);

            entity.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasIndex(r => r.Token)
                .IsUnique();

            entity.Property(r => r.BonusAmount)
                .HasPrecision(18, 2);

            entity.Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(r => r.InviterUser)
                .WithMany(u => u.SentReferrals)
                .HasForeignKey(r => r.InviterUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureOverdraftSetting(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OverdraftSetting>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.InterestRateModifier)
                .HasPrecision(8, 4);

            entity.Property(o => o.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(o => o.CreatedByUser)
                .WithMany()
                .HasForeignKey(o => o.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureVerificationCode(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VerificationCode>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.Property(v => v.Type)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(v => v.Code)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(v => new { v.UserId, v.Type });
        });
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "User" },
            new Role { Id = 2, Name = "Admin" }
        );

        // Seed Currencies
        modelBuilder.Entity<Currency>().HasData(
            new Currency { Id = 1, Code = "EUR", Name = "Euro", ExchangeRate = 1.0m },
            new Currency { Id = 2, Code = "USD", Name = "US Dollar", ExchangeRate = 1.08m },
            new Currency { Id = 3, Code = "GBP", Name = "British Pound", ExchangeRate = 0.86m }
        );

        // Seed default Overdraft Setting
        modelBuilder.Entity<OverdraftSetting>().HasData(
            new OverdraftSetting
            {
                Id = 1,
                InterestRateModifier = 0.05m,
                EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = null,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
