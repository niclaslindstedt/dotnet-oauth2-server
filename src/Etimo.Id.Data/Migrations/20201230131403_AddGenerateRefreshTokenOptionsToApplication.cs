using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddGenerateRefreshTokenOptionsToApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GenerateRefreshTokenForAuthorizationCode",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GenerateRefreshTokenForClientCredentials",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GenerateRefreshTokenForImplicitFlow",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GenerateRefreshTokenForPasswordCredentials",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenerateRefreshTokenForAuthorizationCode",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "GenerateRefreshTokenForClientCredentials",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "GenerateRefreshTokenForImplicitFlow",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "GenerateRefreshTokenForPasswordCredentials",
                table: "Applications");
        }
    }
}
