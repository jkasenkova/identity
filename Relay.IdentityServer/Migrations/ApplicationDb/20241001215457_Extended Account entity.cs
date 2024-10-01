using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relay.IdentityServer.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class ExtendedAccountentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveHandovers",
                schema: "Identity",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "Identity",
                table: "Accounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Identity",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LimitUsers",
                schema: "Identity",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Identity",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TimeZoneId",
                schema: "Identity",
                table: "Accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveHandovers",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LimitUsers",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                schema: "Identity",
                table: "Accounts");
        }
    }
}
