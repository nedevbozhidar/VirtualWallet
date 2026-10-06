IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Currencies] (
    [Id] int NOT NULL IDENTITY,
    [Code] nchar(3) NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [ExchangeRate] decimal(18,6) NOT NULL,
    CONSTRAINT [PK_Currencies] PRIMARY KEY ([Id])
);

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(20) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Salt] nvarchar(max) NOT NULL,
    [Email] nvarchar(254) NOT NULL,
    [PhoneNumber] nchar(10) NOT NULL,
    [PhotoUrl] nvarchar(500) NULL,
    [RoleId] int NOT NULL,
    [IsBlocked] bit NOT NULL,
    [IsEmailVerified] bit NOT NULL,
    [IdentityVerificationStatus] nvarchar(20) NOT NULL,
    [IdCardPhotoUrl] nvarchar(500) NULL,
    [SelfiePhotoUrl] nvarchar(500) NULL,
    [ReferralsUsed] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Cards] (
    [Id] int NOT NULL IDENTITY,
    [CardNumber] nchar(16) NOT NULL,
    [ExpirationDate] datetime2 NOT NULL,
    [CardHolder] nvarchar(30) NOT NULL,
    [CheckNumber] nchar(3) NOT NULL,
    [IsPrimary] bit NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Cards] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cards_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Contacts] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ContactUserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Contacts_Users_ContactUserId] FOREIGN KEY ([ContactUserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Contacts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [OverdraftSettings] (
    [Id] int NOT NULL IDENTITY,
    [InterestRateModifier] decimal(8,4) NOT NULL,
    [EffectiveFrom] datetime2 NOT NULL,
    [CreatedByUserId] int NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_OverdraftSettings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OverdraftSettings_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Referrals] (
    [Id] int NOT NULL IDENTITY,
    [InviterUserId] int NOT NULL,
    [InviteeEmail] nvarchar(254) NOT NULL,
    [Token] nvarchar(128) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsCompleted] bit NOT NULL,
    [BonusAmount] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Referrals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Referrals_Users_InviterUserId] FOREIGN KEY ([InviterUserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [VerificationCodes] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Type] nvarchar(30) NOT NULL,
    [Code] nvarchar(10) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_VerificationCodes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VerificationCodes_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Wallets] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [IsDefault] bit NOT NULL,
    [Type] nvarchar(20) NOT NULL,
    [UserId] int NOT NULL,
    [CurrencyId] int NOT NULL,
    [IsOverdraftEnabled] bit NOT NULL,
    [OverdraftLimit] decimal(18,2) NOT NULL,
    [ConsecutiveNegativeMonths] int NOT NULL,
    [SavingsAmount] decimal(18,2) NULL,
    [SavingsMaturityDate] datetime2 NULL,
    [SavingsInterestRate] decimal(8,4) NULL,
    [SourceWalletId] int NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Wallets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Wallets_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Wallets_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Wallets_Wallets_SourceWalletId] FOREIGN KEY ([SourceWalletId]) REFERENCES [Wallets] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [RecurringTransactions] (
    [Id] int NOT NULL IDENTITY,
    [Amount] decimal(18,2) NOT NULL,
    [Description] nvarchar(500) NULL,
    [SenderWalletId] int NOT NULL,
    [RecipientWalletId] int NOT NULL,
    [CategoryId] int NULL,
    [Interval] nvarchar(20) NOT NULL,
    [NextExecutionDate] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_RecurringTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RecurringTransactions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_RecurringTransactions_Wallets_RecipientWalletId] FOREIGN KEY ([RecipientWalletId]) REFERENCES [Wallets] ([Id]),
    CONSTRAINT [FK_RecurringTransactions_Wallets_SenderWalletId] FOREIGN KEY ([SenderWalletId]) REFERENCES [Wallets] ([Id])
);

CREATE TABLE [Transfers] (
    [Id] int NOT NULL IDENTITY,
    [Amount] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CardId] int NOT NULL,
    [WalletId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Transfers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transfers_Cards_CardId] FOREIGN KEY ([CardId]) REFERENCES [Cards] ([Id]),
    CONSTRAINT [FK_Transfers_Wallets_WalletId] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id])
);

CREATE TABLE [WalletMembers] (
    [Id] int NOT NULL IDENTITY,
    [WalletId] int NOT NULL,
    [UserId] int NOT NULL,
    [CanSpend] bit NOT NULL,
    [CanAddMoney] bit NOT NULL,
    [AddedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_WalletMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WalletMembers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_WalletMembers_Wallets_WalletId] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Transactions] (
    [Id] int NOT NULL IDENTITY,
    [Amount] decimal(18,2) NOT NULL,
    [Description] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [SenderUserId] int NOT NULL,
    [SenderWalletId] int NOT NULL,
    [RecipientUserId] int NOT NULL,
    [RecipientWalletId] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [CategoryId] int NULL,
    [RecurringTransactionId] int NULL,
    [VerificationCode] nvarchar(10) NULL,
    [VerificationCodeExpiry] datetime2 NULL,
    [IsVerified] bit NOT NULL,
    [ExchangeRate] decimal(18,6) NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transactions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Transactions_RecurringTransactions_RecurringTransactionId] FOREIGN KEY ([RecurringTransactionId]) REFERENCES [RecurringTransactions] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Transactions_Users_RecipientUserId] FOREIGN KEY ([RecipientUserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Transactions_Users_SenderUserId] FOREIGN KEY ([SenderUserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Transactions_Wallets_RecipientWalletId] FOREIGN KEY ([RecipientWalletId]) REFERENCES [Wallets] ([Id]),
    CONSTRAINT [FK_Transactions_Wallets_SenderWalletId] FOREIGN KEY ([SenderWalletId]) REFERENCES [Wallets] ([Id])
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'ExchangeRate', N'Name') AND [object_id] = OBJECT_ID(N'[Currencies]'))
    SET IDENTITY_INSERT [Currencies] ON;
INSERT INTO [Currencies] ([Id], [Code], [ExchangeRate], [Name])
VALUES (1, N'EUR', 1.0, N'Euro'),
(2, N'USD', 1.08, N'US Dollar'),
(3, N'GBP', 0.86, N'British Pound');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'ExchangeRate', N'Name') AND [object_id] = OBJECT_ID(N'[Currencies]'))
    SET IDENTITY_INSERT [Currencies] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedByUserId', N'EffectiveFrom', N'InterestRateModifier') AND [object_id] = OBJECT_ID(N'[OverdraftSettings]'))
    SET IDENTITY_INSERT [OverdraftSettings] ON;
INSERT INTO [OverdraftSettings] ([Id], [CreatedAt], [CreatedByUserId], [EffectiveFrom], [InterestRateModifier])
VALUES (1, '2024-01-01T00:00:00.0000000Z', NULL, '2024-01-01T00:00:00.0000000Z', 0.05);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedByUserId', N'EffectiveFrom', N'InterestRateModifier') AND [object_id] = OBJECT_ID(N'[OverdraftSettings]'))
    SET IDENTITY_INSERT [OverdraftSettings] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [Name])
VALUES (1, N'User'),
(2, N'Admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;

CREATE UNIQUE INDEX [IX_Cards_CardNumber] ON [Cards] ([CardNumber]);

CREATE INDEX [IX_Cards_UserId] ON [Cards] ([UserId]);

CREATE UNIQUE INDEX [IX_Categories_UserId_Name] ON [Categories] ([UserId], [Name]);

CREATE INDEX [IX_Contacts_ContactUserId] ON [Contacts] ([ContactUserId]);

CREATE UNIQUE INDEX [IX_Contacts_UserId_ContactUserId] ON [Contacts] ([UserId], [ContactUserId]);

CREATE UNIQUE INDEX [IX_Currencies_Code] ON [Currencies] ([Code]);

CREATE INDEX [IX_OverdraftSettings_CreatedByUserId] ON [OverdraftSettings] ([CreatedByUserId]);

CREATE INDEX [IX_RecurringTransactions_CategoryId] ON [RecurringTransactions] ([CategoryId]);

CREATE INDEX [IX_RecurringTransactions_IsActive] ON [RecurringTransactions] ([IsActive]);

CREATE INDEX [IX_RecurringTransactions_NextExecutionDate] ON [RecurringTransactions] ([NextExecutionDate]);

CREATE INDEX [IX_RecurringTransactions_RecipientWalletId] ON [RecurringTransactions] ([RecipientWalletId]);

CREATE INDEX [IX_RecurringTransactions_SenderWalletId] ON [RecurringTransactions] ([SenderWalletId]);

CREATE INDEX [IX_Referrals_InviterUserId] ON [Referrals] ([InviterUserId]);

CREATE UNIQUE INDEX [IX_Referrals_Token] ON [Referrals] ([Token]);

CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);

CREATE INDEX [IX_Transactions_CategoryId] ON [Transactions] ([CategoryId]);

CREATE INDEX [IX_Transactions_CreatedAt] ON [Transactions] ([CreatedAt]);

CREATE INDEX [IX_Transactions_RecipientUserId] ON [Transactions] ([RecipientUserId]);

CREATE INDEX [IX_Transactions_RecipientWalletId] ON [Transactions] ([RecipientWalletId]);

CREATE INDEX [IX_Transactions_RecurringTransactionId] ON [Transactions] ([RecurringTransactionId]);

CREATE INDEX [IX_Transactions_SenderUserId] ON [Transactions] ([SenderUserId]);

CREATE INDEX [IX_Transactions_SenderWalletId] ON [Transactions] ([SenderWalletId]);

CREATE INDEX [IX_Transactions_Status] ON [Transactions] ([Status]);

CREATE INDEX [IX_Transfers_CardId] ON [Transfers] ([CardId]);

CREATE INDEX [IX_Transfers_CreatedAt] ON [Transfers] ([CreatedAt]);

CREATE INDEX [IX_Transfers_WalletId] ON [Transfers] ([WalletId]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

CREATE UNIQUE INDEX [IX_Users_PhoneNumber] ON [Users] ([PhoneNumber]);

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);

CREATE INDEX [IX_VerificationCodes_UserId_Type] ON [VerificationCodes] ([UserId], [Type]);

CREATE INDEX [IX_WalletMembers_UserId] ON [WalletMembers] ([UserId]);

CREATE UNIQUE INDEX [IX_WalletMembers_WalletId_UserId] ON [WalletMembers] ([WalletId], [UserId]);

CREATE INDEX [IX_Wallets_CurrencyId] ON [Wallets] ([CurrencyId]);

CREATE INDEX [IX_Wallets_SourceWalletId] ON [Wallets] ([SourceWalletId]);

CREATE INDEX [IX_Wallets_UserId] ON [Wallets] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261006092741_InitialCreate', N'10.0.12');

COMMIT;
GO

