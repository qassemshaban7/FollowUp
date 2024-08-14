using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class adminsett : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "DepartmentId", "Email", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b3b3810c-c9fd-4bea-9e3f-3b62841b06fb", 5504, "Admin@gmail.com", "AQAAAAIAAYagAAAAECoFJyF5pevcx7lOHolsFy7h7KqkLr7YnblFdiXgCzsoVqRsUSEp0diy2Wz6sIJDAw==", "b7166976-5a42-433d-af6f-83d87fdcd96c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "DepartmentId", "Email", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0be6e064-ae5e-436a-998c-9a91842039c8", null, null, "AQAAAAIAAYagAAAAEEH1qdqg/rr3m+fPAfHYNKF5FMnPK7sF2sFtu1g/a87sGcOrqEN+yqHDpuY4CmnJBA==", "a1d57498-559d-4470-8072-687bd6791677" });
        }
    }
}
