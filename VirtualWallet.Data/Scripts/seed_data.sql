-- =========================================================================
-- VirtualWallet Seed Data Script
-- Populates the VirtualWalletDb database with initial test data.
-- =========================================================================

USE [VirtualWalletDb];
GO

-- 1. Insert Test Users (Assumes Role 1 = User, Role 2 = Admin)
IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'admin')
BEGIN
    INSERT INTO [Users] 
        ([Username], [PasswordHash], [Salt], [Email], [PhoneNumber], [PhotoUrl], [RoleId], [IsBlocked], [IsEmailVerified], [IdentityVerificationStatus], [ReferralsUsed], [CreatedAt])
    VALUES 
        (N'admin', N'AQAAAAEAACcQAAAAEHASH_ADMIN_SECRET', N'SALT_ADMIN', N'admin@virtualwallet.com', N'0888111111', NULL, 2, 0, 1, N'Approved', 0, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'john_doe')
BEGIN
    INSERT INTO [Users] 
        ([Username], [PasswordHash], [Salt], [Email], [PhoneNumber], [PhotoUrl], [RoleId], [IsBlocked], [IsEmailVerified], [IdentityVerificationStatus], [ReferralsUsed], [CreatedAt])
    VALUES 
        (N'john_doe', N'AQAAAAEAACcQAAAAEHASH_JOHN_SECRET', N'SALT_JOHN', N'john@virtualwallet.com', N'0888222222', NULL, 1, 0, 1, N'Approved', 0, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'jane_smith')
BEGIN
    INSERT INTO [Users] 
        ([Username], [PasswordHash], [Salt], [Email], [PhoneNumber], [PhotoUrl], [RoleId], [IsBlocked], [IsEmailVerified], [IdentityVerificationStatus], [ReferralsUsed], [CreatedAt])
    VALUES 
        (N'jane_smith', N'AQAAAAEAACcQAAAAEHASH_JANE_SECRET', N'SALT_JANE', N'jane@virtualwallet.com', N'0888333333', NULL, 1, 0, 1, N'Approved', 0, GETUTCDATE());
END
GO

-- Declare variables for foreign keys
DECLARE @AdminId INT = (SELECT Id FROM Users WHERE Username = 'admin');
DECLARE @JohnId INT = (SELECT Id FROM Users WHERE Username = 'john_doe');
DECLARE @JaneId INT = (SELECT Id FROM Users WHERE Username = 'jane_smith');
DECLARE @EurId INT = (SELECT Id FROM Currencies WHERE Code = 'EUR');
DECLARE @UsdId INT = (SELECT Id FROM Currencies WHERE Code = 'USD');

-- 2. Insert Credit/Debit Cards
IF NOT EXISTS (SELECT 1 FROM [Cards] WHERE [CardNumber] = N'4111111111111111')
BEGIN
    INSERT INTO [Cards] ([CardNumber], [ExpirationDate], [CardHolder], [CheckNumber], [IsPrimary], [UserId], [CreatedAt])
    VALUES 
        (N'4111111111111111', DATEADD(YEAR, 3, GETUTCDATE()), N'John Doe', N'123', 1, @JohnId, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [Cards] WHERE [CardNumber] = N'4222222222222222')
BEGIN
    INSERT INTO [Cards] ([CardNumber], [ExpirationDate], [CardHolder], [CheckNumber], [IsPrimary], [UserId], [CreatedAt])
    VALUES 
        (N'4222222222222222', DATEADD(YEAR, 2, GETUTCDATE()), N'Jane Smith', N'456', 1, @JaneId, GETUTCDATE());
END
GO

-- Re-declare variables after GO batch separator
DECLARE @AdminId INT = (SELECT Id FROM Users WHERE Username = 'admin');
DECLARE @JohnId INT = (SELECT Id FROM Users WHERE Username = 'john_doe');
DECLARE @JaneId INT = (SELECT Id FROM Users WHERE Username = 'jane_smith');
DECLARE @EurId INT = (SELECT Id FROM Currencies WHERE Code = 'EUR');
DECLARE @UsdId INT = (SELECT Id FROM Currencies WHERE Code = 'USD');

-- 3. Insert Wallets
IF NOT EXISTS (SELECT 1 FROM [Wallets] WHERE [UserId] = @JohnId AND [Name] = N'Main Wallet')
BEGIN
    INSERT INTO [Wallets] ([Name], [Balance], [IsDefault], [Type], [UserId], [CurrencyId], [IsOverdraftEnabled], [OverdraftLimit], [ConsecutiveNegativeMonths], [CreatedAt])
    VALUES 
        (N'Main Wallet', 1000.00, 1, N'Personal', @JohnId, @EurId, 1, 200.00, 0, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [Wallets] WHERE [UserId] = @JaneId AND [Name] = N'Main Wallet')
BEGIN
    INSERT INTO [Wallets] ([Name], [Balance], [IsDefault], [Type], [UserId], [CurrencyId], [IsOverdraftEnabled], [OverdraftLimit], [ConsecutiveNegativeMonths], [CreatedAt])
    VALUES 
        (N'Main Wallet', 500.00, 1, N'Personal', @JaneId, @EurId, 0, 0.00, 0, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [Wallets] WHERE [UserId] = @JohnId AND [Name] = N'USD Savings')
BEGIN
    INSERT INTO [Wallets] ([Name], [Balance], [IsDefault], [Type], [UserId], [CurrencyId], [IsOverdraftEnabled], [OverdraftLimit], [ConsecutiveNegativeMonths], [CreatedAt])
    VALUES 
        (N'USD Savings', 250.00, 0, N'Personal', @JohnId, @UsdId, 0, 0.00, 0, GETUTCDATE());
END
GO

-- 4. Insert Categories
DECLARE @JohnId INT = (SELECT Id FROM Users WHERE Username = 'john_doe');
DECLARE @JaneId INT = (SELECT Id FROM Users WHERE Username = 'jane_smith');

IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [UserId] = @JohnId AND [Name] = N'Groceries')
BEGIN
    INSERT INTO [Categories] ([Name], [UserId]) VALUES (N'Groceries', @JohnId), (N'Utilities', @JohnId), (N'Entertainment', @JohnId);
END

IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [UserId] = @JaneId AND [Name] = N'Restaurants')
BEGIN
    INSERT INTO [Categories] ([Name], [UserId]) VALUES (N'Restaurants', @JaneId), (N'Rent', @JaneId);
END
GO

-- 5. Insert Contacts
DECLARE @JohnId INT = (SELECT Id FROM Users WHERE Username = 'john_doe');
DECLARE @JaneId INT = (SELECT Id FROM Users WHERE Username = 'jane_smith');

IF NOT EXISTS (SELECT 1 FROM [Contacts] WHERE [UserId] = @JohnId AND [ContactUserId] = @JaneId)
BEGIN
    INSERT INTO [Contacts] ([UserId], [ContactUserId], [CreatedAt])
    VALUES (@JohnId, @JaneId, GETUTCDATE());
END
GO

-- 6. Insert Sample Transactions & Top-Up Transfers
DECLARE @JohnId INT = (SELECT Id FROM Users WHERE Username = 'john_doe');
DECLARE @JaneId INT = (SELECT Id FROM Users WHERE Username = 'jane_smith');
DECLARE @JohnWalletId INT = (SELECT Id FROM Wallets WHERE UserId = @JohnId AND IsDefault = 1);
DECLARE @JaneWalletId INT = (SELECT Id FROM Wallets WHERE UserId = @JaneId AND IsDefault = 1);
DECLARE @JohnCardId INT = (SELECT Id FROM Cards WHERE UserId = @JohnId);
DECLARE @GroceriesCategoryId INT = (SELECT Id FROM Categories WHERE UserId = @JohnId AND Name = N'Groceries');

-- Card Top-Up Transfer
IF NOT EXISTS (SELECT 1 FROM [Transfers] WHERE [CardId] = @JohnCardId)
BEGIN
    INSERT INTO [Transfers] ([Amount], [CreatedAt], [CardId], [WalletId], [Status])
    VALUES (200.00, GETUTCDATE(), @JohnCardId, @JohnWalletId, N'Confirmed');
END

-- User-to-User Money Transfer
IF NOT EXISTS (SELECT 1 FROM [Transactions] WHERE [SenderUserId] = @JohnId AND [RecipientUserId] = @JaneId)
BEGIN
    INSERT INTO [Transactions] 
        ([Amount], [Description], [CreatedAt], [SenderUserId], [SenderWalletId], [RecipientUserId], [RecipientWalletId], [Status], [CategoryId], [IsVerified])
    VALUES 
        (45.50, N'Dinner & drinks split', GETUTCDATE(), @JohnId, @JohnWalletId, @JaneId, @JaneWalletId, N'Confirmed', @GroceriesCategoryId, 1);
END
GO
