using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class updateRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68099eeb-4f6b-45c6-9c00-68099eebd485",
                column: "ArabicRoleName",
                value: "مدرب");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9eebd485-2a1d-45c6-9c00-68099eebd485",
                column: "ArabicRoleName",
                value: "مشرف");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba51b8f7-2a1d-45c6-9c00-68099eebd485",
                column: "ArabicRoleName",
                value: "عميد الكلية");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0d845ada-8145-4edb-8ad2-5e25538ff25f", "AQAAAAIAAYagAAAAEKoTfQU8ts6A4E84j2wdvcMEDfHzOfa5sndixNPLRUNT7vGFRJCgFPyEGsyZqJJWEw==", "e225f692-62a1-4d29-9549-2e7be7be7af9" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68099eeb-4f6b-45c6-9c00-68099eebd485",
                column: "ArabicRoleName",
                value: "المدرب");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9eebd485-2a1d-45c6-9c00-68099eebd485",
                column: "ArabicRoleName",
                value: "المشرف");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba51b8f7-2a1d-45c6-9c00-68099eebd485",
                column: "ArabicRoleName",
                value: "الادمن");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d7af983c-2cfa-43e2-ba01-42a2bc989f84", "AQAAAAIAAYagAAAAEPNNcXjgZcnPgdCNPys5jzEHEDSlmGdWSzvCwFhd5VaJwxIADEe6vW6ZgdAEvngSyQ==", "d9ed2783-e008-4bf8-8877-d03fa4391790" });
        }
    }
}
