using EnrollmentManager.API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentManager.API.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260827000000_AddStudentRegistrationAndActiveEnrollmentConstraints")]
public partial class AddStudentRegistrationAndActiveEnrollmentConstraints : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsActiveEnrollment",
            table: "Enrollments",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateIndex(
            name: "IX_Students_RegistrationNumber",
            table: "Students",
            column: "RegistrationNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Enrollments_StudentId_CourseId_FormatId_Active",
            table: "Enrollments",
            columns: new[] { "StudentId", "CourseId", "FormatId" },
            unique: true,
            filter: "\"IsActiveEnrollment\" = TRUE");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Students_RegistrationNumber",
            table: "Students");

        migrationBuilder.DropIndex(
            name: "IX_Enrollments_StudentId_CourseId_FormatId_Active",
            table: "Enrollments");

        migrationBuilder.DropColumn(
            name: "IsActiveEnrollment",
            table: "Enrollments");
    }
}
