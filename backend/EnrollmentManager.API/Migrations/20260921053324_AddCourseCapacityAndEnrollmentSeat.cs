using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseCapacityAndEnrollmentSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConsumesSeat",
                table: "Enrollments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "AvailableSlots",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSlots",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsumesSeat",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "AvailableSlots",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "TotalSlots",
                table: "Courses");
        }
    }
}
