using Microsoft.EntityFrameworkCore.Migrations;

namespace Etimo.Id.Data.Migrations
{
    public partial class AddAllowGrantsToApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowAuthorizationCodeGrant",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowClientCredentialsGrant",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowImplicitGrant",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowResourceOwnerPasswordCredentialsGrant",
                table: "Applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAuthorizationCodeGrant",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "AllowClientCredentialsGrant",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "AllowImplicitGrant",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "AllowResourceOwnerPasswordCredentialsGrant",
                table: "Applications");
        }
    }
}
