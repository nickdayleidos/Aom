using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class breakfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Break1Time",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "Break2Time",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "Break3Time",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "Break4Time",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "Break5Time",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "Break6Time",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "Break7Time",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "Break8Time",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "BreakTime",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "LunchTime",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "ShiftLength",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "WfmComment",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.RenameColumn(
                name: "Breaks",
                schema: "Employee",
                table: "BreakTemplates",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "Employee",
                table: "BreakTemplates",
                newName: "Breaks");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break1Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break2Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break3Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break4Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break5Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break6Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break7Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Break8Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTime",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "BreakTemplates",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "LunchTime",
                schema: "Employee",
                table: "BreakTemplates",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShiftLength",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WfmComment",
                schema: "Employee",
                table: "BreakTemplates",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
