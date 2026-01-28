using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class IsImpacting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImpacting",
                schema: "Employee",
                table: "OperaRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Timeframe",
                schema: "Employee",
                table: "OperaRequest",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImpacting",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropColumn(
                name: "Timeframe",
                schema: "Employee",
                table: "OperaRequest");
        }
    }
}
