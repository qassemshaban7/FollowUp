using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class updateConfig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3b0bba3e-8967-4e31-b8af-abfb52792be1", "AQAAAAIAAYagAAAAEGXZI4KQGQ2ocZJOz7kqugIkccKqY0X1nwA+Ag8WeAzqQjJLK2MZmBpQOfVd7khiPA==", "adf303b9-5f98-429a-b44e-4924ebc24b05" });

            migrationBuilder.InsertData(
                table: "configs",
                columns: new[] { "Id", "Name" },
                values: new object[] { 6, "الكلية التقنية للاتصالات والمعلومات بجدة" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "configs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be0ce0dd-e568-4e23-97c3-9657e04afd00", "AQAAAAIAAYagAAAAEDKsVKV9VcK9wW+xl7dm1PKPJBlCm0CkVHqeAjDZm2D4g7BkLio9pueRinnW/7ec0w==", "646d4d5c-028f-448a-b81b-0c25fa17f140" });
        }
    }
}
