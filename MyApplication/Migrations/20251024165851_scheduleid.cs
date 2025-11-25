using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class scheduleid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_AcrSchedule_ScheduleRequestId",
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

            migrationBuilder.RenameColumn(
                name: "breakTemplateId",
                schema: "Employee",
                table: "AcrSchedule",
                newName: "BreakTemplateId");

            migrationBuilder.AlterColumn<int>(
                name: "BreakTemplateId",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.RenameColumn(
                name: "BreakTemplateId",
                schema: "Employee",
                table: "AcrSchedule",
                newName: "breakTemplateId");

            migrationBuilder.AlterColumn<int>(
                name: "breakTemplateId",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
