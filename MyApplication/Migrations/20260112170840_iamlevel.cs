using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class iamlevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IamLevel",
                schema: "Employee",
                table: "SubOrganization",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IatLevel",
                schema: "Employee",
                table: "SubOrganization",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IamLevel",
                schema: "Employee",
                table: "CertificationTypes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IamLevel",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.DropColumn(
                name: "IatLevel",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.DropColumn(
                name: "IamLevel",
                schema: "Employee",
                table: "CertificationTypes");
        }
    }
}
