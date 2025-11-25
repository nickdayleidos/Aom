using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class sid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_AcrSchedule_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeHistory_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory",
                newName: "SourceAcrId");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevertedTime",
                schema: "Employee",
                table: "AcrRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "ScheduleRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_AcrSchedule_ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "ScheduleRequestId",
                principalSchema: "Employee",
                principalTable: "AcrSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_AcrSchedule_ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeHistory_ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropColumn(
                name: "ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropColumn(
                name: "RevertedTime",
                schema: "Employee",
                table: "AcrRequest");

            migrationBuilder.RenameColumn(
                name: "SourceAcrId",
                schema: "Employee",
                table: "EmployeeHistory",
                newName: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "ScheduleId");

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
    }
}
