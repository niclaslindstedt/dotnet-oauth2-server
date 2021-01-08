using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class ChangeLockedUntilDateToNullableForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LockedUntilDateTime",
                table: "Users",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_UserId",
                table: "AuthorizationCodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationCodes_Users_UserId",
                table: "AuthorizationCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationCodes_Users_UserId",
                table: "AuthorizationCodes");

            migrationBuilder.DropIndex(
                name: "IX_AuthorizationCodes_UserId",
                table: "AuthorizationCodes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LockedUntilDateTime",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }
    }
}
