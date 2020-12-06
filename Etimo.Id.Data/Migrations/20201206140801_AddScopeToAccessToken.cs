using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddScopeToAccessToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "AccessTokens",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                table: "AccessTokens");
        }
    }
}
