using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class operastatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OperaSubClassId",
                schema: "Employee",
                table: "OperaRequest",
                newName: "OperaStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_OperaStatusId",
                schema: "Employee",
                table: "OperaRequest",
                column: "OperaStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperaRequest_OperaStatus_OperaStatusId",
                schema: "Employee",
                table: "OperaRequest",
                column: "OperaStatusId",
                principalSchema: "Employee",
                principalTable: "OperaStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperaRequest_OperaStatus_OperaStatusId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropIndex(
                name: "IX_OperaRequest_OperaStatusId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.RenameColumn(
                name: "OperaStatusId",
                schema: "Employee",
                table: "OperaRequest",
                newName: "OperaSubClassId");
        }
    }
}
