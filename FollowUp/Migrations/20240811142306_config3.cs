using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class config3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b50cfdb5-c861-44ad-8f5c-6b39f08e4240", "AQAAAAIAAYagAAAAEI8D4WBMDv9AXGJ2s0Hjq4t8FMxa+1LGxtjV46fjfV5h3XI/hDUPVzJ1FxpdlujCCg==", "0c856730-0775-48fa-8d45-52c3fee20947" });

            migrationBuilder.InsertData(
                table: "configs",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "مشعل مرزوق العتيبي" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3eccd39b-9f0d-4538-ab1c-181dc928512e", "AQAAAAIAAYagAAAAEIjTBbZ+7BjTGXEU/6CMTzG2plJohNj4S2rYlVf+h+w8guxqC774hEaTQgnW+6czJw==", "864d7e7c-2bad-4883-aed1-495ec54115ae" });
        }
    }
}
