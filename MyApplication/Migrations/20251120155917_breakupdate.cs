using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class breakupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BreakTime",
                schema: "Employee",
                table: "BreakTemplates",
                newName: "LunchLength");

            migrationBuilder.AddColumn<int>(
                name: "BreakLength",
                schema: "Employee",
                table: "BreakTemplates",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakLength",
                schema: "Employee",
                table: "BreakTemplates");

            migrationBuilder.RenameColumn(
                name: "LunchLength",
                schema: "Employee",
                table: "BreakTemplates",
                newName: "BreakTime");
        }
    }
}
