using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class EmailTemplate : Migration
    {
        public string Body { get; internal set; }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwsId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EDIPI",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsRemote",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SubOrganizationId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "UsnEmail",
                table: "Employees",
                newName: "NmciEmail");

            migrationBuilder.RenameColumn(
                name: "LeidosUserName",
                table: "Employees",
                newName: "FlankspeedEmail");

            migrationBuilder.RenameColumn(
                name: "FlankSpeedUserName",
                table: "Employees",
                newName: "DomainLoginName");

            migrationBuilder.AlterColumn<string>(
                name: "CorporateId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NmciEmail",
                table: "Employees",
                newName: "UsnEmail");

            migrationBuilder.RenameColumn(
                name: "FlankspeedEmail",
                table: "Employees",
                newName: "LeidosUserName");

            migrationBuilder.RenameColumn(
                name: "DomainLoginName",
                table: "Employees",
                newName: "FlankSpeedUserName");

            migrationBuilder.AlterColumn<int>(
                name: "CorporateId",
                table: "Employees",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AwsId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EDIPI",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployerId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRemote",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubOrganizationId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "Employees",
                type: "int",
                nullable: true);
        }
    }
}
