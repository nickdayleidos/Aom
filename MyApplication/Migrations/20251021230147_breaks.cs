using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class breaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_Schedule_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropTable(
                name: "Schedule",
                schema: "Employee");

            migrationBuilder.AddColumn<int>(
                name: "breakTemplateId",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BreakSchedules",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeesId = table.Column<int>(type: "int", nullable: true),
                    MondayTemplateId = table.Column<int>(type: "int", nullable: false),
                    TuesdayTemplateId = table.Column<int>(type: "int", nullable: false),
                    WednesdayTemplateId = table.Column<int>(type: "int", nullable: false),
                    ThursdayTemplateId = table.Column<int>(type: "int", nullable: false),
                    FridayTemplateId = table.Column<int>(type: "int", nullable: false),
                    SaturdayTemplateId = table.Column<int>(type: "int", nullable: false),
                    SundayTemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreakSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreakSchedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BreakSchedules_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BreakTemplates",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Breaks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BreakTime = table.Column<int>(type: "int", nullable: false),
                    ShiftLength = table.Column<int>(type: "int", nullable: false),
                    Break1Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    Break2Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    Break3Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    Break4Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    Break5Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    Break6Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    Break7Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    Break8Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    LunchTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    WfmComment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreakTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeSchedules",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duration = table.Column<TimeOnly>(type: "time", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId1 = table.Column<int>(type: "int", nullable: true),
                    BeforeShiftMonday = table.Column<TimeOnly>(type: "time", nullable: false),
                    AfterShiftMonday = table.Column<TimeOnly>(type: "time", nullable: false),
                    BeforeShiftTuesday = table.Column<TimeOnly>(type: "time", nullable: false),
                    AfterShiftTuesday = table.Column<TimeOnly>(type: "time", nullable: false),
                    BeforeShiftWednesday = table.Column<TimeOnly>(type: "time", nullable: false),
                    AfterShiftWednesday = table.Column<TimeOnly>(type: "time", nullable: false),
                    BeforeShiftThursday = table.Column<TimeOnly>(type: "time", nullable: false),
                    AfterShiftThursday = table.Column<TimeOnly>(type: "time", nullable: false),
                    BeforeShiftFriday = table.Column<TimeOnly>(type: "time", nullable: false),
                    AfterShiftFriday = table.Column<TimeOnly>(type: "time", nullable: false),
                    BeforeShiftSaturday = table.Column<TimeOnly>(type: "time", nullable: false),
                    AfterShiftSaturday = table.Column<TimeOnly>(type: "time", nullable: false),
                    BeforeShiftSunday = table.Column<TimeOnly>(type: "time", nullable: false),
                    AfterShiftSunday = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OvertimeSchedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OvertimeSchedules_Employees_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BreakSchedules_EmployeeId",
                schema: "Employee",
                table: "BreakSchedules",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BreakSchedules_EmployeesId",
                schema: "Employee",
                table: "BreakSchedules",
                column: "EmployeesId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeSchedules_EmployeeId",
                schema: "Employee",
                table: "OvertimeSchedules",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeSchedules_EmployeeId1",
                schema: "Employee",
                table: "OvertimeSchedules",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_AcrSchedule_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "ScheduleId",
                principalSchema: "Employee",
                principalTable: "AcrSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_AcrSchedule_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropTable(
                name: "BreakSchedules",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "BreakTemplates",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "OvertimeSchedules",
                schema: "Employee");

            migrationBuilder.DropColumn(
                name: "breakTemplateId",
                schema: "Employee",
                table: "AcrSchedule");

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

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_Schedule_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "ScheduleId",
                principalSchema: "Employee",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
