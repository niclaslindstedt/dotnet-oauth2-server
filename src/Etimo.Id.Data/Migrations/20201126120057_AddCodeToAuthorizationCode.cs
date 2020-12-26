using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddCodeToAuthorizationCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AuthorizationCodes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_Code",
                table: "AuthorizationCodes",
                column: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthorizationCodes_Code",
                table: "AuthorizationCodes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AuthorizationCodes");
        }
    }
}
