using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetSlot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminSeedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$uxw4qVhvc1V861Zv.0ZgEur8SbmYNz7nAslvZpxvGLKPl.SYEoet2");

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "Email", "PasswordHash" },
                values: new object[] { 2, "user@meetslot.local", "$2a$11$.8xuNfskTdFy1lx9r8WR4.jHv9ElMXVy8lMEYhz6sJGrUMhyP5V02" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$hQ4sA2nY5nYjEFfOeL4f9uD8jz6M9H0JQm3M1xwY9dCF2nB4P3sVi");
        }
    }
}
