using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddLogoBase64ToApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoBase64",
                table: "Applications",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoBase64",
                table: "Applications");
        }
    }
}
