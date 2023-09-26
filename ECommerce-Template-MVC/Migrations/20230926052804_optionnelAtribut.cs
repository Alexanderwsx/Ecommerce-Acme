using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce_Template_MVC.Migrations
{
    /// <inheritdoc />
    public partial class optionnelAtribut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "TempCartId",
                table: "ShoppingCarts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTemporary",
                table: "ShoppingCarts",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5d0f18ed-be64-4d95-8a96-3d2604c5c381", null, "Admin", "ADMIN" },
                    { "99abeccb-2c2a-4a49-be54-c7e1349a313d", null, "Individuel", "USER" },
                    { "ad060aac-994a-49e7-92f0-a076b069500c", null, "Employe", "EMPLOYE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5d0f18ed-be64-4d95-8a96-3d2604c5c381");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99abeccb-2c2a-4a49-be54-c7e1349a313d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad060aac-994a-49e7-92f0-a076b069500c");

            migrationBuilder.AlterColumn<string>(
                name: "TempCartId",
                table: "ShoppingCarts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTemporary",
                table: "ShoppingCarts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

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
    }
}
