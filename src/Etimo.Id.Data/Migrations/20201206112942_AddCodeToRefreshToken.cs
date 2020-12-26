using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddCodeToRefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "RefreshTokens",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Code",
                table: "RefreshTokens",
                column: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AuthorizationCodes_Code",
                table: "RefreshTokens",
                column: "Code",
                principalTable: "AuthorizationCodes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AuthorizationCodes_Code",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Code",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "RefreshTokens");
        }
    }
}
