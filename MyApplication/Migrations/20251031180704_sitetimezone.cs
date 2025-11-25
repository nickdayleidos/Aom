using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class sitetimezone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfterShiftFriday",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "AfterShiftMonday",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "AfterShiftSaturday",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "AfterShiftSunday",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "AfterShiftThursday",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "AfterShiftTuesday",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.DropColumn(
                name: "AfterShiftWednesday",
                schema: "Employee",
                table: "OvertimeSchedules");

            migrationBuilder.RenameColumn(
                name: "BeforeShiftWednesday",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "WednesdayTemplate");

            migrationBuilder.RenameColumn(
                name: "BeforeShiftTuesday",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "TuesdayTemplate");

            migrationBuilder.RenameColumn(
                name: "BeforeShiftThursday",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "ThursdayTemplate");

            migrationBuilder.RenameColumn(
                name: "BeforeShiftSunday",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "SundayTemplate");

            migrationBuilder.RenameColumn(
                name: "BeforeShiftSaturday",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "SaturdayTemplate");

            migrationBuilder.RenameColumn(
                name: "BeforeShiftMonday",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "MondayTemplate");

            migrationBuilder.RenameColumn(
                name: "BeforeShiftFriday",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "FridayTemplate");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneIana",
                schema: "Employee",
                table: "Site",
                type: "varchar(64)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneWindows",
                schema: "Employee",
                table: "Site",
                type: "varchar(64)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneIana",
                schema: "Employee",
                table: "Site");

            migrationBuilder.DropColumn(
                name: "TimeZoneWindows",
                schema: "Employee",
                table: "Site");

            migrationBuilder.RenameColumn(
                name: "WednesdayTemplate",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "BeforeShiftWednesday");

            migrationBuilder.RenameColumn(
                name: "TuesdayTemplate",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "BeforeShiftTuesday");

            migrationBuilder.RenameColumn(
                name: "ThursdayTemplate",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "BeforeShiftThursday");

            migrationBuilder.RenameColumn(
                name: "SundayTemplate",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "BeforeShiftSunday");

            migrationBuilder.RenameColumn(
                name: "SaturdayTemplate",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "BeforeShiftSaturday");

            migrationBuilder.RenameColumn(
                name: "MondayTemplate",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "BeforeShiftMonday");

            migrationBuilder.RenameColumn(
                name: "FridayTemplate",
                schema: "Employee",
                table: "OvertimeSchedules",
                newName: "BeforeShiftFriday");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AfterShiftFriday",
                schema: "Employee",
                table: "OvertimeSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AfterShiftMonday",
                schema: "Employee",
                table: "OvertimeSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AfterShiftSaturday",
                schema: "Employee",
                table: "OvertimeSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AfterShiftSunday",
                schema: "Employee",
                table: "OvertimeSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AfterShiftThursday",
                schema: "Employee",
                table: "OvertimeSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AfterShiftTuesday",
                schema: "Employee",
                table: "OvertimeSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "AfterShiftWednesday",
                schema: "Employee",
                table: "OvertimeSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
