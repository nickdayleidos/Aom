using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class impacting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Identifiers_AwsId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Identifiers_Employees_EmployeeId",
                schema: "Aws",
                table: "Identifiers");

            migrationBuilder.DropIndex(
                name: "IX_Identifiers_EmployeeId",
                schema: "Aws",
                table: "Identifiers");

            migrationBuilder.DropIndex(
                name: "IX_Employees_AwsId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.AddColumn<bool>(
                name: "ImpactingOverride",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identifiers_EmployeeId",
                schema: "Aws",
                table: "Identifiers",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Identifiers_Employees_EmployeeId",
                schema: "Aws",
                table: "Identifiers",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identifiers_Employees_EmployeeId",
                schema: "Aws",
                table: "Identifiers");

            migrationBuilder.DropIndex(
                name: "IX_Identifiers_EmployeeId",
                schema: "Aws",
                table: "Identifiers");

            migrationBuilder.DropColumn(
                name: "ImpactingOverride",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.CreateIndex(
                name: "IX_Identifiers_EmployeeId",
                schema: "Aws",
                table: "Identifiers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AwsId",
                schema: "Employee",
                table: "Employees",
                column: "AwsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Identifiers_AwsId",
                schema: "Employee",
                table: "Employees",
                column: "AwsId",
                principalSchema: "Aws",
                principalTable: "Identifiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Identifiers_Employees_EmployeeId",
                schema: "Aws",
                table: "Identifiers",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
