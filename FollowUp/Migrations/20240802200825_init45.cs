using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class init45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3183bd19-83ea-408e-a3c3-9ecaa27c5a2b", "AQAAAAIAAYagAAAAEDDaPfQqdvmKhJs5ob4mREjQKsR++AFRz2vgPfWRX1tb8gitYadidXRzEpD0BN+tFg==", "bf48c924-2c86-4458-bb16-aee5d3702b52" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5565f977-1431-487d-95dc-bf25fa48d216", "AQAAAAIAAYagAAAAEL5yjsAtmjQj9pj4qHslIX4eAqU62Y01JqwWuv/7vMTcr0/zSrx+RphX+mvIKWrzXg==", "361e8290-db6e-42d9-8c43-df6e115f1d20" });
        }
    }
}
