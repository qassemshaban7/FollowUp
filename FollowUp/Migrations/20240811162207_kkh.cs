using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class kkh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0be6e064-ae5e-436a-998c-9a91842039c8", "AQAAAAIAAYagAAAAEEH1qdqg/rr3m+fPAfHYNKF5FMnPK7sF2sFtu1g/a87sGcOrqEN+yqHDpuY4CmnJBA==", "a1d57498-559d-4470-8072-687bd6791677" });

            migrationBuilder.InsertData(
                table: "configs",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "حسين عبده الشاعري" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b50cfdb5-c861-44ad-8f5c-6b39f08e4240", "AQAAAAIAAYagAAAAEI8D4WBMDv9AXGJ2s0Hjq4t8FMxa+1LGxtjV46fjfV5h3XI/hDUPVzJ1FxpdlujCCg==", "0c856730-0775-48fa-8d45-52c3fee20947" });
        }
    }
}
