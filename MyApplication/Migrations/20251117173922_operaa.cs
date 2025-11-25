using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class operaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApproveBy",
                schema: "Employee",
                table: "OperaRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveTime",
                schema: "Employee",
                table: "OperaRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                schema: "Employee",
                table: "OperaRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledTime",
                schema: "Employee",
                table: "OperaRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                schema: "Employee",
                table: "OperaRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedTime",
                schema: "Employee",
                table: "OperaRequest",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveBy",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropColumn(
                name: "ApproveTime",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropColumn(
                name: "CancelledTime",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropColumn(
                name: "RejectedTime",
                schema: "Employee",
                table: "OperaRequest");
        }
    }
}
