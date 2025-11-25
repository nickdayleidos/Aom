using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class awsstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.DropColumn(
                name: "ShortName",
                schema: "Employee",
                table: "Organization");

            migrationBuilder.EnsureSchema(
                name: "Aws");

            migrationBuilder.AddColumn<int>(
                name: "AwsStatusId",
                schema: "Employee",
                table: "SubOrganization",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Statuses",
                schema: "Aws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubOrganization_AwsStatusId",
                schema: "Employee",
                table: "SubOrganization",
                column: "AwsStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubOrganization_Statuses_AwsStatusId",
                schema: "Employee",
                table: "SubOrganization",
                column: "AwsStatusId",
                principalSchema: "Aws",
                principalTable: "Statuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubOrganization_Statuses_AwsStatusId",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.DropTable(
                name: "Statuses",
                schema: "Aws");

            migrationBuilder.DropIndex(
                name: "IX_SubOrganization_AwsStatusId",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.DropColumn(
                name: "AwsStatusId",
                schema: "Employee",
                table: "SubOrganization");

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                schema: "Employee",
                table: "SubOrganization",
                type: "varchar(128)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                schema: "Employee",
                table: "Organization",
                type: "varchar(128)",
                nullable: true);
        }
    }
}
