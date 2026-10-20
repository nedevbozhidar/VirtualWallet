using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualWallet.Data.Migrations;

public partial class ProtectStoredCardData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Cards_CardNumber",
            table: "Cards");

        migrationBuilder.AddColumn<string>(
            name: "ProtectedCardNumber",
            table: "Cards",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "LastFour",
            table: "Cards",
            type: "nchar(4)",
            fixedLength: true,
            maxLength: 4,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new InvalidOperationException(
            "This migration removes stored card numbers and CVVs after backfill. Restore from a pre-migration backup to roll back.");
    }
}
