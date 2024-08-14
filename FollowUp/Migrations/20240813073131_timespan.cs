using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class timespan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "FromTime",
                table: "Permissions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ToTime",
                table: "Permissions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f8d8d0d0-7304-438d-a707-6c0c16833232", "AQAAAAIAAYagAAAAEBH+7KKrLuCkga2nqQn6CQZgWRXAqJ7PA2U2l/MVjwStvVyzDBG2dINZxCktacNEEw==", "ad7278af-980e-43e6-87b2-26f444b7b2e5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromTime",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ToTime",
                table: "Permissions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b3b3810c-c9fd-4bea-9e3f-3b62841b06fb", "AQAAAAIAAYagAAAAECoFJyF5pevcx7lOHolsFy7h7KqkLr7YnblFdiXgCzsoVqRsUSEp0diy2Wz6sIJDAw==", "b7166976-5a42-433d-af6f-83d87fdcd96c" });
        }
    }
}
