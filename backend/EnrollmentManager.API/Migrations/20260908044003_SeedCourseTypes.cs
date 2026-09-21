using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EnrollmentManager.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedCourseTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CourseTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Bacharelado" },
                    { 2, "Tecnólogo" },
                    { 3, "Licenciatura" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CourseTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CourseTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CourseTypes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
