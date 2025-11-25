using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class _202511030936 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOvertimeSchedules_Employees_EmployeeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_AcrRequestId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_EmployeeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.RenameColumn(
                name: "IsOtException",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                newName: "IsOtAdjustment");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftNumber",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsOtAdjustment",
                schema: "Employee",
                table: "AcrSchedule",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OvertimeTypes",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_AcrRequestId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "AcrRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_FridayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "FridayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_MondayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "MondayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_SaturdayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "SaturdayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_SundayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "SundayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_ThursdayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "ThursdayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_TuesdayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "TuesdayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOvertimeSchedules_WednesdayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "WednesdayTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOvertimeSchedules_FridayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "FridayTypeId",
                principalSchema: "Employee",
                principalTable: "OvertimeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOvertimeSchedules_MondayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "MondayTypeId",
                principalSchema: "Employee",
                principalTable: "OvertimeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOvertimeSchedules_SaturdayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "SaturdayTypeId",
                principalSchema: "Employee",
                principalTable: "OvertimeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOvertimeSchedules_SundayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "SundayTypeId",
                principalSchema: "Employee",
                principalTable: "OvertimeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOvertimeSchedules_ThursdayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "ThursdayTypeId",
                principalSchema: "Employee",
                principalTable: "OvertimeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOvertimeSchedules_TuesdayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "TuesdayTypeId",
                principalSchema: "Employee",
                principalTable: "OvertimeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOvertimeSchedules_WednesdayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "WednesdayTypeId",
                principalSchema: "Employee",
                principalTable: "OvertimeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOvertimeSchedules_FridayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOvertimeSchedules_MondayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOvertimeSchedules_SaturdayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOvertimeSchedules_SundayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOvertimeSchedules_ThursdayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOvertimeSchedules_TuesdayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrOvertimeSchedules_WednesdayType_OvertimeTypes",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropTable(
                name: "OvertimeTypes",
                schema: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_AcrRequestId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_FridayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_MondayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_SaturdayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_SundayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_ThursdayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_TuesdayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AcrOvertimeSchedules_WednesdayTypeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "IsOtAdjustment",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.RenameColumn(
                name: "IsOtAdjustment",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                newName: "IsOtException");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftNumber",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOvertimeSchedules_Employees_EmployeeId",
                schema: "Employee",
                table: "AcrOvertimeSchedules",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
