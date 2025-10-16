using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class orgfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Employees_ManagerId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Employees_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Employee",
                table: "SubOrganization",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Employee",
                table: "Organization",
                newName: "ShortName");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                schema: "Employee",
                table: "SubOrganization",
                type: "int",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Manager_ManagerId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Supervisor_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                schema: "Employee",
                table: "SubOrganization",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                schema: "Employee",
                table: "Organization",
                newName: "Description");

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
    }
}
