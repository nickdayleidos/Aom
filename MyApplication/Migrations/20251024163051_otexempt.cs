using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class otexempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOtExempt",
                schema: "Employee",
                table: "EmployeeHistory",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOtExempt",
                schema: "Employee",
                table: "AcrOrganization",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOtExempt",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropColumn(
                name: "IsOtExempt",
                schema: "Employee",
                table: "AcrOrganization");
        }
    }
}
