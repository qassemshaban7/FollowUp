using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class updateConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be0ce0dd-e568-4e23-97c3-9657e04afd00", "AQAAAAIAAYagAAAAEDKsVKV9VcK9wW+xl7dm1PKPJBlCm0CkVHqeAjDZm2D4g7BkLio9pueRinnW/7ec0w==", "646d4d5c-028f-448a-b81b-0c25fa17f140" });

            migrationBuilder.InsertData(
                table: "configs",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "المملكة العربية السعودية       المؤسسة العامة للتدريب التقني والمهني         الكلية التقنية للاتصالات والمعلومات بجدة        إدارة الرقابة " },
                    { 4, "KINGDOM OF SAUDI ARABIA        Technical and Vocational Training Corporation      Technical College of Telecommunications & Info – Jeddah " },
                    { 5, "شعار.jpg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f8d8d0d0-7304-438d-a707-6c0c16833232", "AQAAAAIAAYagAAAAEBH+7KKrLuCkga2nqQn6CQZgWRXAqJ7PA2U2l/MVjwStvVyzDBG2dINZxCktacNEEw==", "ad7278af-980e-43e6-87b2-26f444b7b2e5" });
        }
    }
}
