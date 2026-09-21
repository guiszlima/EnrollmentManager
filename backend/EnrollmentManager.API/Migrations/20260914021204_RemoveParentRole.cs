using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentManager.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveParentRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 5, "PARENT", "Parent" });
        }
    }
}
