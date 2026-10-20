using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Context;
using VirtualWallet.Data.Security;

namespace VirtualWallet.Web.Services;

public static class LegacyCardStorageMigrator
{
    public static async Task ProtectLegacyCardsAsync(
        VirtualWalletDbContext context,
        ICardDataProtector cardDataProtector)
    {
        var connection = context.Database.GetDbConnection();
        var closeConnection = connection.State != System.Data.ConnectionState.Open;
        if (closeConnection)
        {
            await connection.OpenAsync();
        }

        try
        {
            await using var transaction = await connection.BeginTransactionAsync();
            await ExecuteAsync(connection, transaction, """
                DECLARE @lockResult int;
                EXEC @lockResult = sp_getapplock
                    @Resource = N'VirtualWallet.CardStorageMigration',
                    @LockMode = N'Exclusive',
                    @LockOwner = N'Transaction',
                    @LockTimeout = 60000;
                IF @lockResult < 0
                    THROW 51000, 'Could not acquire the card storage migration lock.', 1;
                """);
            var hasLegacyCardNumber = await ColumnExistsAsync(connection, transaction, "CardNumber");

            if (hasLegacyCardNumber)
            {
                await AddColumnIfMissingAsync(connection, transaction,
                    "ProtectedCardNumber", "nvarchar(500) NULL");
                await AddColumnIfMissingAsync(connection, transaction,
                    "LastFour", "nchar(4) NULL");

                var legacyCards = await ReadLegacyCardsAsync(connection, transaction);
                foreach (var legacyCard in legacyCards)
                {
                    var cardNumber = legacyCard.CardNumber.Trim();
                    if (cardNumber.Length != 16 || cardNumber.Any(character => !char.IsDigit(character)))
                    {
                        throw new InvalidOperationException(
                            $"Card {legacyCard.Id} has an invalid legacy card number; it was not migrated.");
                    }

                    await UpdateProtectedCardAsync(
                        connection,
                        transaction,
                        legacyCard.Id,
                        cardDataProtector.Protect(cardNumber),
                        cardNumber[^4..]);
                }

                await ExecuteAsync(connection, transaction, """
                    IF EXISTS (SELECT 1 FROM sys.indexes
                               WHERE name = N'IX_Cards_CardNumber' AND object_id = OBJECT_ID(N'dbo.Cards'))
                        DROP INDEX [IX_Cards_CardNumber] ON [dbo].[Cards];
                    ALTER TABLE [dbo].[Cards] DROP COLUMN [CardNumber], [CheckNumber];
                    """);
            }

            await ExecuteAsync(connection, transaction, """
                ALTER TABLE [dbo].[Cards] ALTER COLUMN [ProtectedCardNumber] nvarchar(500) NOT NULL;
                ALTER TABLE [dbo].[Cards] ALTER COLUMN [LastFour] nchar(4) NOT NULL;
                """);

            await transaction.CommitAsync();
        }
        finally
        {
            if (closeConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task<bool> ColumnExistsAsync(
        DbConnection connection,
        DbTransaction transaction,
        string columnName)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT CASE WHEN COL_LENGTH(N'dbo.Cards', @columnName) IS NULL THEN 0 ELSE 1 END";
        AddParameter(command, "@columnName", columnName);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
    }

    private static async Task AddColumnIfMissingAsync(
        DbConnection connection,
        DbTransaction transaction,
        string columnName,
        string sqlType)
    {
        if (await ColumnExistsAsync(connection, transaction, columnName))
        {
            return;
        }

        await ExecuteAsync(connection, transaction,
            $"ALTER TABLE [dbo].[Cards] ADD [{columnName}] {sqlType};");
    }

    private static async Task<List<(int Id, string CardNumber)>> ReadLegacyCardsAsync(
        DbConnection connection,
        DbTransaction transaction)
    {
        var cards = new List<(int Id, string CardNumber)>();
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT [Id], [CardNumber] FROM [dbo].[Cards]";
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            cards.Add((reader.GetInt32(0), reader.GetString(1)));
        }

        return cards;
    }

    private static async Task UpdateProtectedCardAsync(
        DbConnection connection,
        DbTransaction transaction,
        int cardId,
        string protectedNumber,
        string lastFour)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            UPDATE [dbo].[Cards]
            SET [ProtectedCardNumber] = @protectedNumber,
                [LastFour] = @lastFour
            WHERE [Id] = @cardId;
            """;
        AddParameter(command, "@protectedNumber", protectedNumber);
        AddParameter(command, "@lastFour", lastFour);
        AddParameter(command, "@cardId", cardId);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task ExecuteAsync(
        DbConnection connection,
        DbTransaction transaction,
        string sql)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
