using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class sched : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.RenameColumn(
                name: "isActive",
                schema: "Employee",
                table: "Skills",
                newName: "IsActive");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<int>(
                name: "Minutes",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsImpacting",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<int>(
                name: "AwsStatusId",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_EmployeeId",
                schema: "Employee",
                table: "Skills",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Employees_EmployeeId",
                schema: "Employee",
                table: "Skills",
                column: "EmployeeId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Employees_EmployeeId",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_EmployeeId",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "Employee",
                table: "Skills",
                newName: "isActive");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Minutes",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsImpacting",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "AwsStatusId",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
