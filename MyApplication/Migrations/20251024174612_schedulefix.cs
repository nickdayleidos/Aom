using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class schedulefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_AcrSchedule_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory",
                newName: "ScheduleRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeHistory_ScheduleId",
                schema: "Employee",
                table: "EmployeeHistory",
                newName: "IX_EmployeeHistory_ScheduleRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_AcrRequest_ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "ScheduleRequestId",
                principalSchema: "Employee",
                principalTable: "AcrRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_AcrRequest_ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.RenameColumn(
                name: "ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeHistory_ScheduleRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                newName: "IX_EmployeeHistory_ScheduleId");

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
