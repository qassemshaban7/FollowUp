using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class pro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7d016d53-300c-485c-88b4-f054269afc80", "AQAAAAIAAYagAAAAEPhqdpCL3tsvuXHES3yA3ke72jPH1MGJM9I5U8upFCaZa79udsY7urVLgR2xf5ZL9g==", "27ef3d80-7ec3-4b66-aa24-97b97f89e5bd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "19639102-d264-4502-a26b-3fdd996c955b", "AQAAAAIAAYagAAAAEJfT8UL6LgkN3WlI/X1kt9OTK1xoxcOizcqd58qQ1BMF4u66c2AVqu9pUz2BWxTfeA==", "20308df7-ae8e-4798-abbe-7826d4d798d2" });

            migrationBuilder.InsertData(
                table: "configs",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "المملكة العربية السعودية       المؤسسة العامة للتدريب التقني والمهني         الكلية التقنية للاتصالات والمعلومات بجدة        إدارة الرقابة " },
                    { 4, "KINGDOM OF SAUDI ARABIA        Technical and Vocational Training Corporation      Technical College of Telecommunications & Info – Jeddah " }
                });
        }
    }
}
