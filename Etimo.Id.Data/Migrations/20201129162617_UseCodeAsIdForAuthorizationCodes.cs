using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Etimo.Id.Data.Migrations
{
    public partial class UseCodeAsIdForAuthorizationCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorizationCodes",
                table: "AuthorizationCodes");

            migrationBuilder.DropIndex(
                name: "IX_AuthorizationCodes_Code",
                table: "AuthorizationCodes");

            migrationBuilder.DropColumn(
                name: "AuthorizationCodeId",
                table: "AuthorizationCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AuthorizationCodes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorizationCodes",
                table: "AuthorizationCodes",
                column: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorizationCodes",
                table: "AuthorizationCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AuthorizationCodes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "AuthorizationCodeId",
                table: "AuthorizationCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorizationCodes",
                table: "AuthorizationCodes",
                column: "AuthorizationCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_Code",
                table: "AuthorizationCodes",
                column: "Code");
        }
    }
}
