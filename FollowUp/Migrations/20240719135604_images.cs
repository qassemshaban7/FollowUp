using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FollowUp.Migrations
{
    /// <inheritdoc />
    public partial class images : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "Image", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d91c01e0-01e2-48dd-af4b-a3c0f2989b59", null, "AQAAAAIAAYagAAAAEAjIxEsokmEsm0MurvgFqgvdm/AEoiqz6LCQ1+4yyZa52xKQt960wp/kD9/vpeERiw==", "a6d4aa94-f8f7-47ba-b6a7-47d8326c73b0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d25967ef-b211-44a5-b21e-1c988cc0503c", "AQAAAAIAAYagAAAAEEhwG1k3DPYWBKkDoiZWY+xeRXI+N8xiyw9MwjCGgGPoOYpqnpXuSG7rodKVYM/99A==", "79cecbbc-9938-40c7-bdb4-79fe7a7af4cc" });
        }
    }
}
