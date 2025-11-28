using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class dsupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailedSchedule_AcrRequest_AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropIndex(
                name: "IX_DetailedSchedule_AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropColumn(
                name: "AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_ScheduleRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "ScheduleRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailedSchedule_AcrRequest_ScheduleRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "ScheduleRequestId",
                principalSchema: "Employee",
                principalTable: "AcrRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailedSchedule_AcrRequest_ScheduleRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropIndex(
                name: "IX_DetailedSchedule_ScheduleRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.AddColumn<int>(
                name: "AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "AcrRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailedSchedule_AcrRequest_AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "AcrRequestId",
                principalSchema: "Employee",
                principalTable: "AcrRequest",
                principalColumn: "Id");
        }
    }
}
