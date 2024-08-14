using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class rejectStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Permissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "19639102-d264-4502-a26b-3fdd996c955b", "AQAAAAIAAYagAAAAEJfT8UL6LgkN3WlI/X1kt9OTK1xoxcOizcqd58qQ1BMF4u66c2AVqu9pUz2BWxTfeA==", "20308df7-ae8e-4798-abbe-7826d4d798d2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "819a9755-6674-4610-a2ba-108d20b6783c", "AQAAAAIAAYagAAAAEHIrZJbktJ2n7Uov5hCj4wxiYk4ZiHsVbcK2ltVsGtgIIuakSH4ohVyuez6pBh8LrA==", "fe2e73ce-5910-4446-8d5a-c4d622458f33" });
        }
    }
}
