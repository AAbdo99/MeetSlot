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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$hQ4sA2nY5nYjEFfOeL4f9uD8jz6M9H0JQm3M1xwY9dCF2nB4P3sVi");
        }
    }
}