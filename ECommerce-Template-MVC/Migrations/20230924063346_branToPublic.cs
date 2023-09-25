using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce_Template_MVC.Migrations
{
    /// <inheritdoc />
    public partial class branToPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f5d3e78-3987-4463-89e2-19f6cb2ceb8c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64b66078-a0ae-4897-8166-536ccc7bef4d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71896d6c-51ef-4a4d-9dd1-f0f2fd80f185");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7b29b53c-240f-4abb-8ceb-2e9e28b9e5c2", null, "Individuel", "USER" },
                    { "97a6f67b-4dab-462c-bef0-07ce64e73ab3", null, "Employe", "EMPLOYE" },
                    { "f5258eff-d351-413f-99ae-35d38b790612", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7b29b53c-240f-4abb-8ceb-2e9e28b9e5c2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97a6f67b-4dab-462c-bef0-07ce64e73ab3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f5258eff-d351-413f-99ae-35d38b790612");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Products");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5f5d3e78-3987-4463-89e2-19f6cb2ceb8c", null, "Admin", "ADMIN" },
                    { "64b66078-a0ae-4897-8166-536ccc7bef4d", null, "Individuel", "USER" },
                    { "71896d6c-51ef-4a4d-9dd1-f0f2fd80f185", null, "Employe", "EMPLOYE" }
                });
        }
    }
}
