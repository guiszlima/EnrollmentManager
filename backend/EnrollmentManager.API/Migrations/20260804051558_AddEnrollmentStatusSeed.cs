using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EnrollmentManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentStatusSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EnrollmentStatuses",
                columns: new[] { "Id", "Code", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "PENDING", "Matrícula realizada, aguardando validação de documentos ou pagamento.", "Pendente" },
                    { 2, "APPROVED", "Matrícula ativa e aprovada pela instituição.", "Aprovada" },
                    { 3, "SUSPENDED", "Matrícula temporariamente pausada pelo aluno.", "Trancada" },
                    { 4, "CANCELLED", "Matrícula cancelada antes ou durante o período letivo.", "Cancelada" },
                    { 5, "COMPLETED", "Aluno finalizou com sucesso o programa.", "Concluída" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EnrollmentStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EnrollmentStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EnrollmentStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EnrollmentStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EnrollmentStatuses",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
