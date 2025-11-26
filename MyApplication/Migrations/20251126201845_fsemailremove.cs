using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class fsemailremove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlankspeedEmail",
                schema: "Employee",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlankspeedEmail",
                schema: "Employee",
                table: "Employees",
                type: "varchar(64)",
                nullable: true);
        }
    }
}
