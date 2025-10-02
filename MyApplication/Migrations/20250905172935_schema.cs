using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "OiCategories");

            migrationBuilder.DropTable(
                name: "OiEvents");

            migrationBuilder.DropTable(
                name: "OiEventUpdates");

            migrationBuilder.DropTable(
                name: "OiSeverities");

            migrationBuilder.DropTable(
                name: "ProactiveAnnouncements");

            migrationBuilder.EnsureSchema(
                name: "Tools");

            migrationBuilder.EnsureSchema(
                name: "Employee");

            migrationBuilder.RenameTable(
                name: "Supervisor",
                newName: "Supervisor",
                newSchema: "Employee");

            migrationBuilder.RenameTable(
                name: "SubOrganization",
                newName: "SubOrganization",
                newSchema: "Employee");

            migrationBuilder.RenameTable(
                name: "Site",
                newName: "Site",
                newSchema: "Employee");

            migrationBuilder.RenameTable(
                name: "Organization",
                newName: "Organization",
                newSchema: "Employee");

            migrationBuilder.RenameTable(
                name: "OiStatus",
                newName: "OiStatus",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "Manager",
                newName: "Manager",
                newSchema: "Employee");

            migrationBuilder.RenameTable(
                name: "IntervalSummary",
                newName: "IntervalSummary",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "Employer",
                newName: "Employer",
                newSchema: "Employee");

            migrationBuilder.RenameTable(
                name: "EmailTemplates",
                newName: "EmailTemplates",
                newSchema: "Tools");

            migrationBuilder.CreateTable(
                name: "Employee",
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
                    DomainLoginName = table.Column<string>(type: "varchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeHistory",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SupervisorId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    EmployerId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    SubOrganizationId = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsLoa = table.Column<bool>(type: "bit", nullable: false),
                    IsIntLoa = table.Column<bool>(type: "bit", nullable: false),
                    IsRemote = table.Column<bool>(type: "bit", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalSchema: "Employee",
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Manager_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "Employee",
                        principalTable: "Manager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Employee",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_SubOrganization_SubOrganizationId",
                        column: x => x.SubOrganizationId,
                        principalSchema: "Employee",
                        principalTable: "SubOrganization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalSchema: "Employee",
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OiCategory",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OiEvent",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SeverityId = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicesAffected = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsersAffected = table.Column<int>(type: "int", nullable: false),
                    EstimatedTimeToResolve = table.Column<string>(type: "varchar(64)", nullable: true),
                    PostedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolutionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OiEventUpdate",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiEventUpdate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OiSeverity",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiSeverity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProactiveAnnouncement",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProactiveTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsnInjectionAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsnSiteAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsnStatusAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProactiveAnnouncement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_EmployerId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_ManagerId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_OrganizationId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_SubOrganizationId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "SubOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_SupervisorId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "SupervisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "EmployeeHistory",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "OiCategory",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "OiEvent",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "OiEventUpdate",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "OiSeverity",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "ProactiveAnnouncement",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "Schedule",
                schema: "Employee");

            migrationBuilder.RenameTable(
                name: "Supervisor",
                schema: "Employee",
                newName: "Supervisor");

            migrationBuilder.RenameTable(
                name: "SubOrganization",
                schema: "Employee",
                newName: "SubOrganization");

            migrationBuilder.RenameTable(
                name: "Site",
                schema: "Employee",
                newName: "Site");

            migrationBuilder.RenameTable(
                name: "Organization",
                schema: "Employee",
                newName: "Organization");

            migrationBuilder.RenameTable(
                name: "OiStatus",
                schema: "Tools",
                newName: "OiStatus");

            migrationBuilder.RenameTable(
                name: "Manager",
                schema: "Employee",
                newName: "Manager");

            migrationBuilder.RenameTable(
                name: "IntervalSummary",
                schema: "Tools",
                newName: "IntervalSummary");

            migrationBuilder.RenameTable(
                name: "Employer",
                schema: "Employee",
                newName: "Employer");

            migrationBuilder.RenameTable(
                name: "EmailTemplates",
                schema: "Tools",
                newName: "EmailTemplates");

            migrationBuilder.CreateTable(
                name: "Employees",
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
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OiCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OiEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedTimeToResolve = table.Column<string>(type: "varchar(64)", nullable: true),
                    PostedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolutionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServicesAffected = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeverityId = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsersAffected = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OiEventUpdates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiEventUpdates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OiSeverities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiSeverities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProactiveAnnouncements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProactiveTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsnInjectionAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsnSiteAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsnStatusAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProactiveAnnouncements", x => x.Id);
                });
        }
    }
}
