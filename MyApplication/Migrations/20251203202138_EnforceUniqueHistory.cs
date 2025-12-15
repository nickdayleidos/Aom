using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class EnforceUniqueHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeHistory_EmployeeId_EffectiveDate",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.RenameTable(
                name: "EmployeeHistory",
                schema: "Employee",
                newName: "EmployeeHistory");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_EmployeeId_EffectiveDate",
                table: "EmployeeHistory",
                columns: new[] { "EmployeeId", "EffectiveDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeHistory_EmployeeId_EffectiveDate",
                table: "EmployeeHistory");

            migrationBuilder.RenameTable(
                name: "EmployeeHistory",
                newName: "EmployeeHistory",
                newSchema: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_EmployeeId_EffectiveDate",
                schema: "Employee",
                table: "EmployeeHistory",
                columns: new[] { "EmployeeId", "EffectiveDate" });
        }
    }
}
