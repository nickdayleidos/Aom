using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class removesubclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperaRequest_OperaSubClass_OperaSubClassId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropTable(
                name: "OperaSubClass",
                schema: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_OperaRequest_OperaSubClassId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropColumn(
                name: "Desc",
                schema: "Employee",
                table: "ActivityType");

            migrationBuilder.DropColumn(
                name: "Desc",
                schema: "Employee",
                table: "ActivitySubType");

            migrationBuilder.DropColumn(
                name: "IsImpacting",
                schema: "Employee",
                table: "ActivitySubType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Desc",
                schema: "Employee",
                table: "ActivityType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Desc",
                schema: "Employee",
                table: "ActivitySubType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsImpacting",
                schema: "Employee",
                table: "ActivitySubType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OperaSubClass",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivitySubTypeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaSubClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperaSubClass_ActivitySubType_ActivitySubTypeId",
                        column: x => x.ActivitySubTypeId,
                        principalSchema: "Employee",
                        principalTable: "ActivitySubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_OperaSubClassId",
                schema: "Employee",
                table: "OperaRequest",
                column: "OperaSubClassId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaSubClass_ActivitySubTypeId",
                schema: "Employee",
                table: "OperaSubClass",
                column: "ActivitySubTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperaRequest_OperaSubClass_OperaSubClassId",
                schema: "Employee",
                table: "OperaRequest",
                column: "OperaSubClassId",
                principalSchema: "Employee",
                principalTable: "OperaSubClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
