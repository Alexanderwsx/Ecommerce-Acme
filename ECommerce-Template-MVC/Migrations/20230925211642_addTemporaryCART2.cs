using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce_Template_MVC.Migrations
{
    /// <inheritdoc />
    public partial class addTemporaryCART2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b437f79-9303-43af-a690-f6b51c9ea037");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "54058294-1ae9-4f00-b7d1-ce96e259382c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "afef2451-4888-4f9f-927b-60aa0782c044");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ShoppingCarts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5c5bdc13-ae89-4d21-8c44-913876b134d3", null, "Individuel", "USER" },
                    { "6c1b571c-971c-4e9a-9f5f-9a58602e3f05", null, "Employe", "EMPLOYE" },
                    { "a449dee3-0739-4917-b81a-e7a01a96771d", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c5bdc13-ae89-4d21-8c44-913876b134d3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c1b571c-971c-4e9a-9f5f-9a58602e3f05");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a449dee3-0739-4917-b81a-e7a01a96771d");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ShoppingCarts");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0b437f79-9303-43af-a690-f6b51c9ea037", null, "Individuel", "USER" },
                    { "54058294-1ae9-4f00-b7d1-ce96e259382c", null, "Admin", "ADMIN" },
                    { "afef2451-4888-4f9f-927b-60aa0782c044", null, "Employe", "EMPLOYE" }
                });
        }
    }
}
