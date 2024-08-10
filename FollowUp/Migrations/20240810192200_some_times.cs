using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class sometimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "81c5657d-1465-4880-a9ef-ff05d88115ec", "AQAAAAIAAYagAAAAEIYGCbvr8ZpE9YLJLWIAfZ5m7J7CHyhoJWUJBNspVxgdCyMPmxgyqIQOQ+POzYnzWw==", "6277491f-a7cd-45a2-9d92-811d8894f57c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0d845ada-8145-4edb-8ad2-5e25538ff25f", "AQAAAAIAAYagAAAAEKoTfQU8ts6A4E84j2wdvcMEDfHzOfa5sndixNPLRUNT7vGFRJCgFPyEGsyZqJJWEw==", "e225f692-62a1-4d29-9549-2e7be7be7af9" });
        }
    }
}
