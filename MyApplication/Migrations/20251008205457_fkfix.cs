using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class fkfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Employer_EmployerId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Manager_ManagerId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Organization_OrganizationId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Site_SiteId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_SubOrganization_SubOrganizationId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Supervisor_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_Employees_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Employer_EmployerId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "EmployerId",
                principalSchema: "Employee",
                principalTable: "Employer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Manager_ManagerId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "ManagerId",
                principalSchema: "Employee",
                principalTable: "Manager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Organization_OrganizationId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "OrganizationId",
                principalSchema: "Employee",
                principalTable: "Organization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Site_SiteId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SiteId",
                principalSchema: "Employee",
                principalTable: "Site",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_SubOrganization_SubOrganizationId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SubOrganizationId",
                principalSchema: "Employee",
                principalTable: "SubOrganization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Supervisor_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SupervisorId",
                principalSchema: "Employee",
                principalTable: "Supervisor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_Employees_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Employer_EmployerId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Manager_ManagerId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Organization_OrganizationId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Site_SiteId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_SubOrganization_SubOrganizationId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_Supervisor_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_Employees_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Employer_EmployerId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "EmployerId",
                principalSchema: "Employee",
                principalTable: "Employer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Manager_ManagerId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "ManagerId",
                principalSchema: "Employee",
                principalTable: "Manager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Organization_OrganizationId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "OrganizationId",
                principalSchema: "Employee",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Site_SiteId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SiteId",
                principalSchema: "Employee",
                principalTable: "Site",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_SubOrganization_SubOrganizationId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SubOrganizationId",
                principalSchema: "Employee",
                principalTable: "SubOrganization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_Supervisor_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SupervisorId",
                principalSchema: "Employee",
                principalTable: "Supervisor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_Employees_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
