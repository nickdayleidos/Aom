using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class staticbreak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FridayTemplateId",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "MondayTemplateId",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "SaturdayTemplateId",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "SundayTemplateId",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "ThursdayTemplateId",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "BreakTemplateId",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.DropColumn(
                name: "BreakTime",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.DropColumn(
                name: "Breaks",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.DropColumn(
                name: "LunchTime",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.RenameColumn(
                name: "WednesdayTemplateId",
                schema: "Employee",
                table: "BreakSchedules",
                newName: "LunchLengh");

            migrationBuilder.RenameColumn(
                name: "TuesdayTemplateId",
                schema: "Employee",
                table: "BreakSchedules",
                newName: "BreakLength");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break1Time",
                schema: "Employee",
                table: "BreakSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break2Time",
                schema: "Employee",
                table: "BreakSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break3Time",
                schema: "Employee",
                table: "BreakSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "LunchTime",
                schema: "Employee",
                table: "BreakSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "isFriday",
                schema: "Employee",
                table: "BreakSchedules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isMonday",
                schema: "Employee",
                table: "BreakSchedules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isSaturday",
                schema: "Employee",
                table: "BreakSchedules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isSunday",
                schema: "Employee",
                table: "BreakSchedules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isThursday",
                schema: "Employee",
                table: "BreakSchedules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isTuesday",
                schema: "Employee",
                table: "BreakSchedules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isWednesday",
                schema: "Employee",
                table: "BreakSchedules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStaticBreakSchedule",
                schema: "Employee",
                table: "AcrSchedule",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Break1Time",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "Break2Time",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "Break3Time",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "LunchTime",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "isFriday",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "isMonday",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "isSaturday",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "isSunday",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "isThursday",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "isTuesday",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "isWednesday",
                schema: "Employee",
                table: "BreakSchedules");

            migrationBuilder.DropColumn(
                name: "IsStaticBreakSchedule",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.RenameColumn(
                name: "LunchLengh",
                schema: "Employee",
                table: "BreakSchedules",
                newName: "WednesdayTemplateId");

            migrationBuilder.RenameColumn(
                name: "BreakLength",
                schema: "Employee",
                table: "BreakSchedules",
                newName: "TuesdayTemplateId");

            migrationBuilder.AddColumn<int>(
                name: "FridayTemplateId",
                schema: "Employee",
                table: "BreakSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayTemplateId",
                schema: "Employee",
                table: "BreakSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayTemplateId",
                schema: "Employee",
                table: "BreakSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayTemplateId",
                schema: "Employee",
                table: "BreakSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayTemplateId",
                schema: "Employee",
                table: "BreakSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakTemplateId",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTime",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Breaks",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LunchTime",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: true);
        }
    }
}
