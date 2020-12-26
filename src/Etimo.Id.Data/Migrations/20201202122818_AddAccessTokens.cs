using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddAccessTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccessTokenId",
                table: "RefreshTokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "RefreshTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "AccessTokenId",
                table: "AuthorizationCodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccessTokens",
                columns: table => new
                {
                    AccessTokenId = table.Column<Guid>(type: "uuid", nullable: false),
                    Disabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokens", x => x.AccessTokenId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AccessTokenId",
                table: "RefreshTokens",
                column: "AccessTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_AccessTokenId",
                table: "AuthorizationCodes",
                column: "AccessTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationCodes_AccessTokens_AccessTokenId",
                table: "AuthorizationCodes",
                column: "AccessTokenId",
                principalTable: "AccessTokens",
                principalColumn: "AccessTokenId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AccessTokens_AccessTokenId",
                table: "RefreshTokens",
                column: "AccessTokenId",
                principalTable: "AccessTokens",
                principalColumn: "AccessTokenId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationCodes_AccessTokens_AccessTokenId",
                table: "AuthorizationCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AccessTokens_AccessTokenId",
                table: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "AccessTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_AccessTokenId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_AuthorizationCodes_AccessTokenId",
                table: "AuthorizationCodes");

            migrationBuilder.DropColumn(
                name: "AccessTokenId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Used",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "AccessTokenId",
                table: "AuthorizationCodes");
        }
    }
}
