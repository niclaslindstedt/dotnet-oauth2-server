using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddCustomQueryParametersToApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowCustomQueryParametersInRedirectUri",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowCustomQueryParametersInRedirectUri",
                table: "Applications");
        }
    }
}
