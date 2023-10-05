using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce_Template_MVC.Migrations
{
    /// <inheritdoc />
    public partial class addShippingFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ea1f443-97f4-4dc9-b330-e84497a95733");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "38126ab7-aa88-4a9f-8885-c797a369174e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce6f11b0-3940-4e5d-a874-381e9a64f9b7");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingAmount",
                table: "OrderHeaders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "41d5b512-4a7e-4e63-9437-1cbea4875acd", null, "Employe", "Employe" },
                    { "715afd5d-fe01-4afe-bd74-dd9bd2227a78", null, "User", "User" },
                    { "b8585294-bbae-4986-8d3e-064d34590617", null, "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "41d5b512-4a7e-4e63-9437-1cbea4875acd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "715afd5d-fe01-4afe-bd74-dd9bd2227a78");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8585294-bbae-4986-8d3e-064d34590617");

            migrationBuilder.DropColumn(
                name: "ShippingAmount",
                table: "OrderHeaders");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1ea1f443-97f4-4dc9-b330-e84497a95733", null, "User", "User" },
                    { "38126ab7-aa88-4a9f-8885-c797a369174e", null, "Admin", "Admin" },
                    { "ce6f11b0-3940-4e5d-a874-381e9a64f9b7", null, "Employe", "Employe" }
                });
        }
    }
}
