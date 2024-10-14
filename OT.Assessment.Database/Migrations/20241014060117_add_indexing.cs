using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OT.Assessment.Database.Migrations
{
    /// <inheritdoc />
    public partial class add_indexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "CasinoWager",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWager_CreatedDateTime",
                table: "CasinoWager",
                column: "CreatedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWager_TransactionId",
                table: "CasinoWager",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWager_WagerId",
                table: "CasinoWager",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CasinoWager_CreatedDateTime",
                table: "CasinoWager");

            migrationBuilder.DropIndex(
                name: "IX_CasinoWager_TransactionId",
                table: "CasinoWager");

            migrationBuilder.DropIndex(
                name: "IX_CasinoWager_WagerId",
                table: "CasinoWager");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "CasinoWager",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
