using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddScopeToAuthorizationCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "AuthorizationCodes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_Name",
                table: "Scopes",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Scopes_Name",
                table: "Scopes");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "AuthorizationCodes");
        }
    }
}
