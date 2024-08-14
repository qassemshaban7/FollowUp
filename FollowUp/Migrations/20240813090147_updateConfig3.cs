using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class updateConfig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "819a9755-6674-4610-a2ba-108d20b6783c", "AQAAAAIAAYagAAAAEHIrZJbktJ2n7Uov5hCj4wxiYk4ZiHsVbcK2ltVsGtgIIuakSH4ohVyuez6pBh8LrA==", "fe2e73ce-5910-4446-8d5a-c4d622458f33" });

            migrationBuilder.InsertData(
                table: "configs",
                columns: new[] { "Id", "Name" },
                values: new object[] { 7, "logo-1.png" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3b0bba3e-8967-4e31-b8af-abfb52792be1", "AQAAAAIAAYagAAAAEGXZI4KQGQ2ocZJOz7kqugIkccKqY0X1nwA+Ag8WeAzqQjJLK2MZmBpQOfVd7khiPA==", "adf303b9-5f98-429a-b44e-4924ebc24b05" });
        }
    }
}
