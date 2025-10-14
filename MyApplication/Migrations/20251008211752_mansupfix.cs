using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class mansupfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Manager_ManagerId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Supervisor_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Employees_ManagerId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "ManagerId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Employees_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SupervisorId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Employees_ManagerId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Employees_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Manager_ManagerId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "ManagerId",
                principalSchema: "Employee",
                principalTable: "Manager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Supervisor_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SupervisorId",
                principalSchema: "Employee",
                principalTable: "Supervisor",
                principalColumn: "Id");
        }
    }
}
