using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relay.IdentityServer.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class Reworkedtimezone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "Identity",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "Identity",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                schema: "Identity",
                table: "Accounts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.AddColumn<Guid>(
                name: "TimeZoneId",
                schema: "Identity",
                table: "Accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
