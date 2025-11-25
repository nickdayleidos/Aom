using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class ot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OvertimeSchedules",
                schema: "Employee");

            migrationBuilder.CreateTable(
                name: "AcrOvertimeSchedules",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcrRequestId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    IsOtException = table.Column<bool>(type: "bit", nullable: false),
                    MondayTypeId = table.Column<int>(type: "int", nullable: true),
                    TuesdayTypeId = table.Column<int>(type: "int", nullable: true),
                    WednesdayTypeId = table.Column<int>(type: "int", nullable: true),
                    ThursdayTypeId = table.Column<int>(type: "int", nullable: true),
                    FridayTypeId = table.Column<int>(type: "int", nullable: true),
                    SaturdayTypeId = table.Column<int>(type: "int", nullable: true),
                    SundayTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcrOvertimeSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcrOvertimeSchedules_AcrRequest_AcrRequestId",
                        column: x => x.AcrRequestId,
                        principalSchema: "Employee",
                        principalTable: "AcrRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcrOvertimeSchedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_AcrRequestId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "AcrRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_EmployeeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcrOvertimeSchedules",
                schema: "Employee");

            migrationBuilder.CreateTable(
                name: "OvertimeSchedules",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duration = table.Column<TimeOnly>(type: "time", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FridayTemplate = table.Column<TimeOnly>(type: "time", nullable: false),
                    MondayTemplate = table.Column<TimeOnly>(type: "time", nullable: false),
                    SaturdayTemplate = table.Column<TimeOnly>(type: "time", nullable: false),
                    SundayTemplate = table.Column<TimeOnly>(type: "time", nullable: false),
                    ThursdayTemplate = table.Column<TimeOnly>(type: "time", nullable: false),
                    TuesdayTemplate = table.Column<TimeOnly>(type: "time", nullable: false),
                    WednesdayTemplate = table.Column<TimeOnly>(type: "time", nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeSchedules_EmployeeId",
                schema: "Employee",
                table: "OvertimeSchedules",
                column: "EmployeeId");
        }
    }
}
