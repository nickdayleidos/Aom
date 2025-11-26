using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class updatedetailedschedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OperaRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "AcrRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_OperaRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "OperaRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailedSchedule_AcrRequest_AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "AcrRequestId",
                principalSchema: "Employee",
                principalTable: "AcrRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailedSchedule_OperaRequest_OperaRequestId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "OperaRequestId",
                principalSchema: "Employee",
                principalTable: "OperaRequest",
                principalColumn: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailedSchedule_AcrRequest_AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_DetailedSchedule_OperaRequest_OperaRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropIndex(
                name: "IX_DetailedSchedule_AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropIndex(
                name: "IX_DetailedSchedule_OperaRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropColumn(
                name: "AcrRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropColumn(
                name: "OperaRequestId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropColumn(
                name: "ScheduleRequestId",
                schema: "Employee",
                table: "DetailedSchedule");
        }
    }
}
