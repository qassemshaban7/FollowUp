using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class config22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3eccd39b-9f0d-4538-ab1c-181dc928512e", "AQAAAAIAAYagAAAAEIjTBbZ+7BjTGXEU/6CMTzG2plJohNj4S2rYlVf+h+w8guxqC774hEaTQgnW+6czJw==", "864d7e7c-2bad-4883-aed1-495ec54115ae" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configs");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2f7d447f-d908-4a8e-a212-75ca732fb069", "AQAAAAIAAYagAAAAEFmAhXs/pqbOWQSsE9hKReMmr6Q9eSo/uMqq4Joh3HUB5ZFo/keOaGr1fLI+JfnjVA==", "2ee0b1ba-378b-45df-92c0-4ac2fd004bfa" });
        }
    }
}
