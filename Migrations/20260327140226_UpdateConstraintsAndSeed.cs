using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MeetSlot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConstraintsAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { 1, "admin@meetslot.local", "$2a$12$hQ4sA2nY5nYjEFfOeL4f9uD8jz6M9H0JQm3M1xwY9dCF2nB4P3sVi", 1 });

            migrationBuilder.InsertData(
                table: "MeetingRooms",
                columns: new[] { "Id", "Capacity", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 4, "Lite rom for standup og korte møter.", "Nordic Room" },
                    { 2, 8, "Mellomstort rom for teammøter.", "Fjord Room" },
                    { 3, 12, "Stort rom for workshops og planlegging.", "Aurora Room" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MeetingRooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MeetingRooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MeetingRooms",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
