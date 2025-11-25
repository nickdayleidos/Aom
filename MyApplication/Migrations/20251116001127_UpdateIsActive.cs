using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOtExempt",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "Supervisor",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "SubOrganization",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "Site",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "OvertimeTypes",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "Organization",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "ActivityType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "ActivitySubType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "OperaSubClass",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "OperaStatus",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "Manager",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "Employer",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "BreakTemplates",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "AcrType",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "AcrStatus",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "Site");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "OvertimeTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "ActivityType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "ActivitySubType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "OperaSubClass");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "OperaStatus");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "Employer");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "AcrType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "AcrStatus");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "Supervisor",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "SubOrganization",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "Organization",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "Manager",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOtExempt",
                schema: "Employee",
                table: "EmployeeHistory",
                type: "bit",
                nullable: true);
        }
    }
}
