using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class manageremployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manager_Employees_EmployeeId",
                schema: "Employee",
                table: "Manager");

            migrationBuilder.AddForeignKey(
                name: "FK_Manager_Employees_EmployeeId",
                schema: "Employee",
                table: "Manager",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manager_Employees_EmployeeId",
                schema: "Employee",
                table: "Manager");

            migrationBuilder.AddForeignKey(
                name: "FK_Manager_Employees_EmployeeId",
                schema: "Employee",
                table: "Manager",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
