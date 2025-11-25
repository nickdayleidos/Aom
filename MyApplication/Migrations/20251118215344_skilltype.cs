using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class skilltype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillTypes_SkillTypeId",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillTypes_SkillTypesId",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillTypes",
                schema: "Employee",
                table: "SkillTypes");

            migrationBuilder.RenameTable(
                name: "SkillTypes",
                schema: "Employee",
                newName: "SkillType",
                newSchema: "Employee");

            migrationBuilder.RenameColumn(
                name: "SkillTypesId",
                schema: "Employee",
                table: "Skills",
                newName: "SkillTypeId1");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_SkillTypesId",
                schema: "Employee",
                table: "Skills",
                newName: "IX_Skills_SkillTypeId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillType",
                schema: "Employee",
                table: "SkillType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillType_SkillTypeId",
                schema: "Employee",
                table: "Skills",
                column: "SkillTypeId",
                principalSchema: "Employee",
                principalTable: "SkillType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillType_SkillTypeId1",
                schema: "Employee",
                table: "Skills",
                column: "SkillTypeId1",
                principalSchema: "Employee",
                principalTable: "SkillType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillType_SkillTypeId",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillType_SkillTypeId1",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillType",
                schema: "Employee",
                table: "SkillType");

            migrationBuilder.RenameTable(
                name: "SkillType",
                schema: "Employee",
                newName: "SkillTypes",
                newSchema: "Employee");

            migrationBuilder.RenameColumn(
                name: "SkillTypeId1",
                schema: "Employee",
                table: "Skills",
                newName: "SkillTypesId");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_SkillTypeId1",
                schema: "Employee",
                table: "Skills",
                newName: "IX_Skills_SkillTypesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillTypes",
                schema: "Employee",
                table: "SkillTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillTypes_SkillTypeId",
                schema: "Employee",
                table: "Skills",
                column: "SkillTypeId",
                principalSchema: "Employee",
                principalTable: "SkillTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillTypes_SkillTypesId",
                schema: "Employee",
                table: "Skills",
                column: "SkillTypesId",
                principalSchema: "Employee",
                principalTable: "SkillTypes",
                principalColumn: "Id");
        }
    }
}
