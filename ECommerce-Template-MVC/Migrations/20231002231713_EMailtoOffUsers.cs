using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce_Template_MVC.Migrations
{
    /// <inheritdoc />
    public partial class EMailtoOffUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12d34b26-bc2b-4368-b815-bdc5b476a545");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d0c45bf-a456-41de-a8da-1214160b751a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "761aba83-1ac4-4dc3-bd2f-8896f924da27");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1ed4a7b2-89c5-4a7c-aedd-289a5d937609", null, "User", "User" },
                    { "9813861c-df1f-4e39-80d4-69837013127c", null, "Employe", "Employe" },
                    { "c8280eb5-732a-43af-bb30-b7310a06ea68", null, "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ed4a7b2-89c5-4a7c-aedd-289a5d937609");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9813861c-df1f-4e39-80d4-69837013127c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8280eb5-732a-43af-bb30-b7310a06ea68");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "OrderHeaders");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "12d34b26-bc2b-4368-b815-bdc5b476a545", null, "User", "User" },
                    { "1d0c45bf-a456-41de-a8da-1214160b751a", null, "Employe", "Employe" },
                    { "761aba83-1ac4-4dc3-bd2f-8896f924da27", null, "Admin", "Admin" }
                });
        }
    }
}
