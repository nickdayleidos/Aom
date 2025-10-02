using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class addmigrationacrcontextaomdbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_Organization_OrganizationId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropTable(
                name: "Employee",
                schema: "Employee");

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                schema: "Employee",
                table: "Supervisor",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubOrganizationId",
                schema: "Employee",
                table: "SubOrganization",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                schema: "Employee",
                table: "Site",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                schema: "Employee",
                table: "Organization",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeesId",
                schema: "Employee",
                table: "OperaRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                schema: "Employee",
                table: "Manager",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployerId",
                schema: "Employee",
                table: "Employer",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                schema: "Employee",
                table: "EmployeeHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "varchar(64)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(64)", nullable: false),
                    MiddleInitial = table.Column<string>(type: "varchar(1)", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    NmciEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    UsnOperatorId = table.Column<string>(type: "varchar(64)", nullable: true),
                    UsnAdminId = table.Column<string>(type: "varchar(64)", nullable: true),
                    FlankspeedEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    CorporateEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    CorporateId = table.Column<string>(type: "varchar(32)", nullable: true),
                    DomainLoginName = table.Column<string>(type: "varchar(32)", nullable: true),
                    EmployeesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_SupervisorId",
                schema: "Employee",
                table: "Supervisor",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_SubOrganization_SubOrganizationId",
                schema: "Employee",
                table: "SubOrganization",
                column: "SubOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Site_SiteId",
                schema: "Employee",
                table: "Site",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_OrganizationId",
                schema: "Employee",
                table: "Organization",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_EmployeesId",
                schema: "Employee",
                table: "OperaRequest",
                column: "EmployeesId");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_ManagerId",
                schema: "Employee",
                table: "Manager",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employer_EmployerId",
                schema: "Employee",
                table: "Employer",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeesId",
                schema: "Employee",
                table: "Employees",
                column: "EmployeesId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_Employees_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_Organization_OrganizationId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "OrganizationId",
                principalSchema: "Employee",
                principalTable: "Organization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employer_Employer_EmployerId",
                schema: "Employee",
                table: "Employer",
                column: "EmployerId",
                principalSchema: "Employee",
                principalTable: "Employer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Manager_Manager_ManagerId",
                schema: "Employee",
                table: "Manager",
                column: "ManagerId",
                principalSchema: "Employee",
                principalTable: "Manager",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperaRequest_Employees_EmployeesId",
                schema: "Employee",
                table: "OperaRequest",
                column: "EmployeesId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_Organization_OrganizationId",
                schema: "Employee",
                table: "Organization",
                column: "OrganizationId",
                principalSchema: "Employee",
                principalTable: "Organization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Site_Site_SiteId",
                schema: "Employee",
                table: "Site",
                column: "SiteId",
                principalSchema: "Employee",
                principalTable: "Site",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubOrganization_SubOrganization_SubOrganizationId",
                schema: "Employee",
                table: "SubOrganization",
                column: "SubOrganizationId",
                principalSchema: "Employee",
                principalTable: "SubOrganization",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Supervisor_Supervisor_SupervisorId",
                schema: "Employee",
                table: "Supervisor",
                column: "SupervisorId",
                principalSchema: "Employee",
                principalTable: "Supervisor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_Employees_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_Organization_OrganizationId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Employer_Employer_EmployerId",
                schema: "Employee",
                table: "Employer");

            migrationBuilder.DropForeignKey(
                name: "FK_Manager_Manager_ManagerId",
                schema: "Employee",
                table: "Manager");

            migrationBuilder.DropForeignKey(
                name: "FK_OperaRequest_Employees_EmployeesId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_Organization_OrganizationId",
                schema: "Employee",
                table: "Organization");

            migrationBuilder.DropForeignKey(
                name: "FK_Site_Site_SiteId",
                schema: "Employee",
                table: "Site");

            migrationBuilder.DropForeignKey(
                name: "FK_SubOrganization_SubOrganization_SubOrganizationId",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_Supervisor_Supervisor_SupervisorId",
                schema: "Employee",
                table: "Supervisor");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Supervisor_SupervisorId",
                schema: "Employee",
                table: "Supervisor");

            migrationBuilder.DropIndex(
                name: "IX_SubOrganization_SubOrganizationId",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.DropIndex(
                name: "IX_Site_SiteId",
                schema: "Employee",
                table: "Site");

            migrationBuilder.DropIndex(
                name: "IX_Organization_OrganizationId",
                schema: "Employee",
                table: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_OperaRequest_EmployeesId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropIndex(
                name: "IX_Manager_ManagerId",
                schema: "Employee",
                table: "Manager");

            migrationBuilder.DropIndex(
                name: "IX_Employer_EmployerId",
                schema: "Employee",
                table: "Employer");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeHistory_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                schema: "Employee",
                table: "Supervisor");

            migrationBuilder.DropColumn(
                name: "SubOrganizationId",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.DropColumn(
                name: "SiteId",
                schema: "Employee",
                table: "Site");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "Employee",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "EmployeesId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                schema: "Employee",
                table: "Manager");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                schema: "Employee",
                table: "Employer");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                schema: "Employee",
                table: "EmployeeHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Employee",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorporateEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    CorporateId = table.Column<string>(type: "varchar(32)", nullable: true),
                    DomainLoginName = table.Column<string>(type: "varchar(32)", nullable: true),
                    FirstName = table.Column<string>(type: "varchar(64)", nullable: false),
                    FlankspeedEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    LastName = table.Column<string>(type: "varchar(64)", nullable: false),
                    MiddleInitial = table.Column<string>(type: "varchar(1)", nullable: true),
                    NmciEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: true),
                    UsnAdminId = table.Column<string>(type: "varchar(64)", nullable: true),
                    UsnOperatorId = table.Column<string>(type: "varchar(64)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_Organization_OrganizationId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "OrganizationId",
                principalSchema: "Employee",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperaRequest_Employee_EmployeeId",
                schema: "Employee",
                table: "OperaRequest",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
