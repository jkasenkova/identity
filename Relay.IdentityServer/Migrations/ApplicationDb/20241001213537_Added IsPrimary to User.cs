using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relay.IdentityServer.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddedIsPrimarytoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                schema: "Identity",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                schema: "Identity",
                table: "AspNetUsers");
        }
    }
}
