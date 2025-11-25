using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class schedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailedSchedule",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ActivityTypeId = table.Column<int>(type: "int", nullable: false),
                    ActivitySubTypeId = table.Column<int>(type: "int", nullable: false),
                    AwsStatusId = table.Column<int>(type: "int", nullable: false),
                    Minutes = table.Column<int>(type: "int", nullable: false),
                    IsImpacting = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailedSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailedSchedule_ActivitySubType_ActivitySubTypeId",
                        column: x => x.ActivitySubTypeId,
                        principalSchema: "Employee",
                        principalTable: "ActivitySubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetailedSchedule_ActivityType_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalSchema: "Employee",
                        principalTable: "ActivityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetailedSchedule_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillTypes",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeesId = table.Column<int>(type: "int", nullable: true),
                    SkillTypeId = table.Column<int>(type: "int", nullable: false),
                    SkillTypesId = table.Column<int>(type: "int", nullable: true),
                    SkillDate = table.Column<DateOnly>(type: "date", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Skills_SkillTypes_SkillTypeId",
                        column: x => x.SkillTypeId,
                        principalSchema: "Employee",
                        principalTable: "SkillTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skills_SkillTypes_SkillTypesId",
                        column: x => x.SkillTypesId,
                        principalSchema: "Employee",
                        principalTable: "SkillTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_ActivitySubTypeId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "ActivitySubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_ActivityTypeId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_EmployeeId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_EmployeesId",
                schema: "Employee",
                table: "Skills",
                column: "EmployeesId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SkillTypeId",
                schema: "Employee",
                table: "Skills",
                column: "SkillTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SkillTypesId",
                schema: "Employee",
                table: "Skills",
                column: "SkillTypesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailedSchedule",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "Skills",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "SkillTypes",
                schema: "Employee");
        }
    }
}
