using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreakSchedules_Employees_EmployeesId",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeSchedules_Employees_EmployeeId1",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_OvertimeSchedules_EmployeeId1",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_BreakSchedules_EmployeesId",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "EmployeesId",
                schema: "Employee",
                table: "BreakSchedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                schema: "Employee",
                table: "OvertimeSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeesId",
                schema: "Employee",
                table: "BreakSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeSchedules_EmployeeId1",
                schema: "Employee",
                table: "OvertimeSchedules",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_BreakSchedules_EmployeesId",
                schema: "Employee",
                table: "BreakSchedules",
                column: "EmployeesId");

            migrationBuilder.AddForeignKey(
                name: "FK_BreakSchedules_Employees_EmployeesId",
                schema: "Employee",
                table: "BreakSchedules",
                column: "EmployeesId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeSchedules_Employees_EmployeeId1",
                schema: "Employee",
                table: "OvertimeSchedules",
                column: "EmployeeId1",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
