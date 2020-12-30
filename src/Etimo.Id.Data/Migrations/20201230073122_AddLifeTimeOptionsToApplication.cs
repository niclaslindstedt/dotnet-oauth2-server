using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddLifeTimeOptionsToApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessTokenLifetimeMinutes",
                table: "Applications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuthorizationCodeLifetimeSeconds",
                table: "Applications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenLifetimeDays",
                table: "Applications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessTokenLifetimeMinutes",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "AuthorizationCodeLifetimeSeconds",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "RefreshTokenLifetimeDays",
                table: "Applications");
        }
    }
}
