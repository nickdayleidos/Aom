using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class acr2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentVmOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprUaifOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprGdaOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprEmailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentEssOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentEmailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentCustomerCareOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmValidationOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmValidationFailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaUsnManOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaSocManOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaAutoOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmEmailBuildoutsOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmCxSatOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmAfuOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlRdmUsnOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlRdmUsnEsdOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprReadyOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprHoldOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprFatalOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprReadyOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprHoldOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprFatalOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AcrStatus",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcrStatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcrStatus_AcrStatus_AcrStatusId",
                        column: x => x.AcrStatusId,
                        principalSchema: "Employee",
                        principalTable: "AcrStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AcrType",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcrTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcrType_AcrType_AcrTypeId",
                        column: x => x.AcrTypeId,
                        principalSchema: "Employee",
                        principalTable: "AcrType",
                        principalColumn: "Id");
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
                    SubmitterCommment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WfmComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcrStatusId = table.Column<int>(type: "int", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcrRequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcrRequest_AcrRequest_AcrRequestId",
                        column: x => x.AcrRequestId,
                        principalSchema: "Employee",
                        principalTable: "AcrRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AcrRequest_AcrStatus_AcrStatusId",
                        column: x => x.AcrStatusId,
                        principalSchema: "Employee",
                        principalTable: "AcrStatus",
                        principalColumn: "Id");
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AcrOrganization_Manager_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "Employee",
                        principalTable: "Manager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AcrOrganization_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Employee",
                        principalTable: "Organization",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AcrOrganization_SubOrganization_SubOrganizationId",
                        column: x => x.SubOrganizationId,
                        principalSchema: "Employee",
                        principalTable: "SubOrganization",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AcrOrganization_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalSchema: "Employee",
                        principalTable: "Supervisor",
                        principalColumn: "Id");
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
                    ShiftNumber = table.Column<bool>(type: "bit", nullable: true),
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
                name: "IX_AcrRequest_AcrRequestId",
                schema: "Employee",
                table: "AcrRequest",
                column: "AcrRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrRequest_AcrStatusId",
                schema: "Employee",
                table: "AcrRequest",
                column: "AcrStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrSchedule_AcrRequestId",
                schema: "Employee",
                table: "AcrSchedule",
                column: "AcrRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrStatus_AcrStatusId",
                schema: "Employee",
                table: "AcrStatus",
                column: "AcrStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrType_AcrTypeId",
                schema: "Employee",
                table: "AcrType",
                column: "AcrTypeId");
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
                name: "AcrType",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "AcrRequest",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "AcrStatus",
                schema: "Employee");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentVmOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprUaifOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprGdaOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprEmailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentEssOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentEmailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentCustomerCareOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmValidationOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmValidationFailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaUsnManOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaSocManOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaAutoOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmEmailBuildoutsOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmCxSatOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmAfuOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlRdmUsnOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlRdmUsnEsdOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprReadyOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprHoldOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprFatalOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprReadyOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprHoldOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprFatalOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);
        }
    }
}
