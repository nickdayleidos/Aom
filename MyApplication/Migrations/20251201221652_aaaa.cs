using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class aaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supervisor_Employees_EmployeeId1",
                schema: "Employee",
                table: "Supervisor");

            migrationBuilder.DropIndex(
                name: "IX_Supervisor_EmployeeId1",
                schema: "Employee",
                table: "Supervisor");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                schema: "Employee",
                table: "Supervisor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                schema: "Employee",
                table: "Supervisor",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_EmployeeId1",
                schema: "Employee",
                table: "Supervisor",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Supervisor_Employees_EmployeeId1",
                schema: "Employee",
                table: "Supervisor",
                column: "EmployeeId1",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
