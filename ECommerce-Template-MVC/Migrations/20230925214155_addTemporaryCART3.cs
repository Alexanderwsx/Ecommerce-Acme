using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce_Template_MVC.Migrations
{
    /// <inheritdoc />
    public partial class addTemporaryCART3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "84202ff8-d649-49a9-98f2-6326eca8c1d9", null, "Admin", "ADMIN" },
                    { "9dbaf121-ca45-44a7-a17e-7f6071fa3ae3", null, "Individuel", "USER" },
                    { "c1451212-3ac1-46c2-9826-a4ae66306649", null, "Employe", "EMPLOYE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84202ff8-d649-49a9-98f2-6326eca8c1d9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9dbaf121-ca45-44a7-a17e-7f6071fa3ae3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c1451212-3ac1-46c2-9826-a4ae66306649");

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
    }
}
