using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListing.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixRoleConcurrencyStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8d139698-a775-4684-abf8-2765e9bd24ce",
                column: "ConcurrencyStamp",
                value: "8d139698-a775-4684-abf8-2765e9bd24ce");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef3013de-7dad-4310-813b-d7d4486874db",
                column: "ConcurrencyStamp",
                value: "ef3013de-7dad-4310-813b-d7d4486874db");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8d139698-a775-4684-abf8-2765e9bd24ce",
                column: "ConcurrencyStamp",
                value: "0213e2d7-6bb8-44d5-bdb7-dbfc5018d10d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef3013de-7dad-4310-813b-d7d4486874db",
                column: "ConcurrencyStamp",
                value: "845e826d-44ce-4acf-ae94-5429e3604cbf");
        }
    }
}
