using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class breaksfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Break1Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Break2Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Break3Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Break4Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Break5Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Break6Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Break7Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Break8Time",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTime",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Breaks",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "BreakTemplates",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LunchTime",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShiftLength",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Breaks",
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
        }
    }
}
