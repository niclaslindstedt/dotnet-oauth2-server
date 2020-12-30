using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddGrantTypeToRefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GrantType",
                table: "RefreshTokens",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrantType",
                table: "RefreshTokens");
        }
    }
}
