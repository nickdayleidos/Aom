using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class isactive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "AcrRequest",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Employee",
                table: "AcrRequest");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Employee",
                table: "AcrOrganization",
                type: "bit",
                nullable: true);
        }
    }
}
