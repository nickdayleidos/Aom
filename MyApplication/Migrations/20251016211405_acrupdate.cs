using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class acrupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WfmComment",
                schema: "Employee",
                table: "AcrRequest",
                type: "varchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubmitterComment",
                schema: "Employee",
                table: "AcrRequest",
                type: "varchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                schema: "Employee",
                table: "AcrRequest",
                type: "varchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerApprovedBy",
                schema: "Employee",
                table: "AcrRequest",
                type: "varchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                schema: "Employee",
                table: "AcrRequest",
                type: "varchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedBy",
                schema: "Employee",
                table: "AcrRequest",
                type: "varchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WfmApprovedBy",
                schema: "Employee",
                table: "AcrRequest",
                type: "varchar(32)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledBy",
                schema: "Employee",
                table: "AcrRequest");

            migrationBuilder.DropColumn(
                name: "ManagerApprovedBy",
                schema: "Employee",
                table: "AcrRequest");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                schema: "Employee",
                table: "AcrRequest");

            migrationBuilder.DropColumn(
                name: "SubmittedBy",
                schema: "Employee",
                table: "AcrRequest");

            migrationBuilder.DropColumn(
                name: "WfmApprovedBy",
                schema: "Employee",
                table: "AcrRequest");

            migrationBuilder.AlterColumn<string>(
                name: "WfmComment",
                schema: "Employee",
                table: "AcrRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubmitterComment",
                schema: "Employee",
                table: "AcrRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldNullable: true);
        }
    }
}
