using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AdjustInModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Secretary");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Student" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_Cpf",
                table: "Students",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_PassportNumber",
                table: "Students",
                column: "PassportNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_Cpf",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_PassportNumber",
                table: "Students");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Student");
        }
    }
}
