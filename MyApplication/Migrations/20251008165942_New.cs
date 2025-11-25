using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class New : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Employee");

            migrationBuilder.EnsureSchema(
                name: "Tools");

            migrationBuilder.CreateTable(
                name: "AcrStatus",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcrType",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "varchar(64)", nullable: false),
                    Subject = table.Column<string>(type: "varchar(128)", nullable: false),
                    ToAddresses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CcAddresses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendFromAddress = table.Column<string>(type: "varchar(64)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

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
                    NmciEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    UsnOperatorId = table.Column<string>(type: "varchar(64)", nullable: true),
                    UsnAdminId = table.Column<string>(type: "varchar(64)", nullable: true),
                    FlankspeedEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    CorporateEmail = table.Column<string>(type: "varchar(64)", nullable: true),
                    CorporateId = table.Column<string>(type: "varchar(32)", nullable: true),
                    DomainLoginName = table.Column<string>(type: "varchar(32)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employer",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntervalSummary",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntervalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IntervalStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    IntervalEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    CurrentUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentUsnASA = table.Column<int>(type: "int", nullable: false),
                    CurrentUsnCallsOffered = table.Column<int>(type: "int", nullable: false),
                    CurrentUsnCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    CurrentVipASA = table.Column<int>(type: "int", nullable: false),
                    CurrentVipCallsOffered = table.Column<int>(type: "int", nullable: false),
                    CurrentVipCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprASA = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprCallsOffered = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    CurrentNnpiASA = table.Column<int>(type: "int", nullable: false),
                    CurrentNnpiCallsOffered = table.Column<int>(type: "int", nullable: false),
                    CurrentNnpiCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    MtdUsnASA = table.Column<int>(type: "int", nullable: false),
                    MtdUsnCallsOffered = table.Column<int>(type: "int", nullable: false),
                    MtdUsnCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    MtdVipASA = table.Column<int>(type: "int", nullable: false),
                    MtdVipCallsOffered = table.Column<int>(type: "int", nullable: false),
                    MtdVipCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    MtdSiprASA = table.Column<int>(type: "int", nullable: false),
                    MtdSiprCallsOffered = table.Column<int>(type: "int", nullable: false),
                    MtdSiprCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    MtdNnpiASA = table.Column<int>(type: "int", nullable: false),
                    MtdNnpiCallsOffered = table.Column<int>(type: "int", nullable: false),
                    MtdNnpiCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    Slr33EmMtdLos1 = table.Column<int>(type: "int", nullable: false),
                    Slr33EmMtdLos2 = table.Column<int>(type: "int", nullable: false),
                    Slr33VmMtdLos1 = table.Column<int>(type: "int", nullable: false),
                    Slr33VmMtdLos2 = table.Column<int>(type: "int", nullable: false),
                    CurrentEmailCount = table.Column<int>(type: "int", nullable: false),
                    CurrentEmailOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CurrentCustomerCareCount = table.Column<int>(type: "int", nullable: false),
                    CurrentCustomerCareOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CurrentSiprEmailCount = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprEmailOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CurrentSiprGdaSpreadsheets = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprGdaOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CurrentSiprUaifCount = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprUaifOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CurrentVmCount = table.Column<int>(type: "int", nullable: false),
                    CurrentVmOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CurrentEssCount = table.Column<int>(type: "int", nullable: false),
                    CurrentEssOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlSrmUaAutoCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmUaAutoOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlSrmUaUsnManCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmUaUsnManOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlSrmUaSocManCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmUaSocManOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlSrmValidationCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmValidationOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlSrmValidationFailCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmValidationFailOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlSrmEmailBuildoutsCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmEmailBuildoutsOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlSrmAfuCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmAfuOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlSrmCxSatCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmCxSatOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlOcmNiprReadyCount = table.Column<int>(type: "int", nullable: false),
                    BlOcmNiprReadyOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlOcmSiprReadyCount = table.Column<int>(type: "int", nullable: false),
                    BlOcmSiprReadyOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlOcmNiprHoldCount = table.Column<int>(type: "int", nullable: false),
                    BlOcmNiprHoldOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlOcmSiprHoldCount = table.Column<int>(type: "int", nullable: false),
                    BlOcmSiprHoldOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlOcmNiprFatalCount = table.Column<int>(type: "int", nullable: false),
                    BlOcmNiprFatalOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlOcmSiprFatalCount = table.Column<int>(type: "int", nullable: false),
                    BlOcmSiprFatalOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlRdmUsnCount = table.Column<int>(type: "int", nullable: false),
                    BlRdmUsnOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    BlRdmUsnEsdCount = table.Column<int>(type: "int", nullable: false),
                    BlRdmUsnEsdOldest = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    NaTodaysFocusArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaMajorCirImpact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaImpactingEvents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaHpsmStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaManagementNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntervalSummary", x => x.Id);
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
                name: "OiStatus",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OiStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperaStatus",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityType",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: true),
                    Description = table.Column<string>(type: "varchar(128)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Site",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "varchar(64)", nullable: true),
                    SiteCode = table.Column<string>(type: "varchar(4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubOrganization",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: true),
                    Description = table.Column<string>(type: "varchar(128)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubOrganization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcrRequest",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AcrTypeId = table.Column<int>(type: "int", nullable: false),
                    AcrStatusId = table.Column<int>(type: "int", nullable: false),
                    SubmitterComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WfmComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcrRequest_AcrStatus_AcrStatusId",
                        column: x => x.AcrStatusId,
                        principalSchema: "Employee",
                        principalTable: "AcrStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcrRequest_AcrType_AcrTypeId",
                        column: x => x.AcrTypeId,
                        principalSchema: "Employee",
                        principalTable: "AcrType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcrRequest_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Manager",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manager_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Supervisor",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supervisor_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivitySubType",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityTypeId = table.Column<int>(type: "int", nullable: false),
                    IsImpacting = table.Column<bool>(type: "bit", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySubType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySubType_ActivityType_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalSchema: "Employee",
                        principalTable: "ActivityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcrSchedule",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcrRequestId = table.Column<int>(type: "int", nullable: false),
                    IsSplitSchedule = table.Column<bool>(type: "bit", nullable: true),
                    ShiftNumber = table.Column<int>(type: "int", nullable: false),
                    MondayStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    MondayEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    TuesdayStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    TuesdayEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    WednesdayStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    WednesdayEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    ThursdayStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    ThursdayEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    FridayStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    FridayEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    SaturdayStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    SaturdayEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    SundayStart = table.Column<TimeOnly>(type: "time", nullable: true),
                    SundayEnd = table.Column<TimeOnly>(type: "time", nullable: true),
                    BreakTime = table.Column<int>(type: "int", nullable: true),
                    Breaks = table.Column<int>(type: "int", nullable: true),
                    LunchTime = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcrSchedule_AcrRequest_AcrRequestId",
                        column: x => x.AcrRequestId,
                        principalSchema: "Employee",
                        principalTable: "AcrRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcrOrganization",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcrRequestId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    SupervisorId = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    SubOrganizationId = table.Column<int>(type: "int", nullable: true),
                    EmployerId = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsLoa = table.Column<bool>(type: "bit", nullable: true),
                    IsIntLoa = table.Column<bool>(type: "bit", nullable: true),
                    IsRemote = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrOrganization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcrOrganization_AcrRequest_AcrRequestId",
                        column: x => x.AcrRequestId,
                        principalSchema: "Employee",
                        principalTable: "AcrRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcrOrganization_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalSchema: "Employee",
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcrOrganization_Manager_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "Employee",
                        principalTable: "Manager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcrOrganization_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Employee",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcrOrganization_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "Employee",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcrOrganization_SubOrganization_SubOrganizationId",
                        column: x => x.SubOrganizationId,
                        principalSchema: "Employee",
                        principalTable: "SubOrganization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcrOrganization_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalSchema: "Employee",
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    EmployerId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_EmployeeHistory_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalSchema: "Employee",
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Manager_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "Employee",
                        principalTable: "Manager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Employee",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalSchema: "Employee",
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "Employee",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_SubOrganization_SubOrganizationId",
                        column: x => x.SubOrganizationId,
                        principalSchema: "Employee",
                        principalTable: "SubOrganization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeHistory_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalSchema: "Employee",
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperaSubClass",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivitySubTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaSubClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperaSubClass_ActivitySubType_ActivitySubTypeId",
                        column: x => x.ActivitySubTypeId,
                        principalSchema: "Employee",
                        principalTable: "ActivitySubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperaRequest",
                schema: "Employee",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityTypeId = table.Column<int>(type: "int", nullable: false),
                    ActivitySubTypeId = table.Column<int>(type: "int", nullable: false),
                    OperaSubClassId = table.Column<int>(type: "int", nullable: true),
                    SubmitterComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedWfm = table.Column<bool>(type: "bit", nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WfmComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaRequest", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_OperaRequest_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperaRequest_OperaSubClass_OperaSubClassId",
                        column: x => x.OperaSubClassId,
                        principalSchema: "Employee",
                        principalTable: "OperaSubClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperaRequest_ActivitySubType_ActivitySubTypeId",
                        column: x => x.ActivitySubTypeId,
                        principalSchema: "Employee",
                        principalTable: "ActivitySubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperaRequest_ActivityType_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalSchema: "Employee",
                        principalTable: "ActivityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_AcrRequestId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "AcrRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_EmployerId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_ManagerId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_OrganizationId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_SiteId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_SubOrganizationId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SubOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_SupervisorId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrRequest_AcrStatusId",
                schema: "Employee",
                table: "AcrRequest",
                column: "AcrStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrRequest_AcrTypeId",
                schema: "Employee",
                table: "AcrRequest",
                column: "AcrTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrRequest_EffectiveDate",
                schema: "Employee",
                table: "AcrRequest",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_AcrRequest_EmployeeId_EffectiveDate",
                schema: "Employee",
                table: "AcrRequest",
                columns: new[] { "EmployeeId", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AcrSchedule_AcrRequestId",
                schema: "Employee",
                table: "AcrSchedule",
                column: "AcrRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_EmployeeId_EffectiveDate",
                schema: "Employee",
                table: "EmployeeHistory",
                columns: new[] { "EmployeeId", "EffectiveDate" });

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
                name: "IX_EmployeeHistory_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_SiteId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "SiteId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Manager_EmployeeId",
                schema: "Employee",
                table: "Manager",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_EmployeeId_StartTime",
                schema: "Employee",
                table: "OperaRequest",
                columns: new[] { "EmployeeId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_OperaSubClassId",
                schema: "Employee",
                table: "OperaRequest",
                column: "OperaSubClassId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_ActivitySubTypeId",
                schema: "Employee",
                table: "OperaRequest",
                column: "ActivitySubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_ActivityTypeId",
                schema: "Employee",
                table: "OperaRequest",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_StartTime",
                schema: "Employee",
                table: "OperaRequest",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_OperaSubClass_ActivitySubTypeId",
                schema: "Employee",
                table: "OperaSubClass",
                column: "ActivitySubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySubType_ActivityTypeId",
                schema: "Employee",
                table: "ActivitySubType",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_EmployeeId",
                schema: "Employee",
                table: "Supervisor",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcrOrganization",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "AcrSchedule",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "EmailTemplates",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "EmployeeHistory",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "IntervalSummary",
                schema: "Tools");

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
                name: "OiStatus",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "OperaRequest",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "OperaStatus",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "ProactiveAnnouncement",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "AcrRequest",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "Employer",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "Manager",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "Organization",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "Schedule",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "Site",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "SubOrganization",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "Supervisor",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "OperaSubClass",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "AcrStatus",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "AcrType",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "ActivitySubType",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "ActivityType",
                schema: "Employee");
        }
    }
}
