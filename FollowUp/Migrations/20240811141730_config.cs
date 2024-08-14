using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2f7d447f-d908-4a8e-a212-75ca732fb069", "AQAAAAIAAYagAAAAEFmAhXs/pqbOWQSsE9hKReMmr6Q9eSo/uMqq4Joh3HUB5ZFo/keOaGr1fLI+JfnjVA==", "2ee0b1ba-378b-45df-92c0-4ac2fd004bfa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "81c5657d-1465-4880-a9ef-ff05d88115ec", "AQAAAAIAAYagAAAAEIYGCbvr8ZpE9YLJLWIAfZ5m7J7CHyhoJWUJBNspVxgdCyMPmxgyqIQOQ+POzYnzWw==", "6277491f-a7cd-45a2-9d92-811d8894f57c" });
        }
    }
}
