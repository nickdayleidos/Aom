using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class skillstypefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CallQueue_Skills_SkillId",
                schema: "Aws",
                table: "CallQueue");

            migrationBuilder.RenameColumn(
                name: "SkillId",
                schema: "Aws",
                table: "CallQueue",
                newName: "SkillTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_CallQueue_SkillId",
                schema: "Aws",
                table: "CallQueue",
                newName: "IX_CallQueue_SkillTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CallQueue_SkillType_SkillTypeId",
                schema: "Aws",
                table: "CallQueue",
                column: "SkillTypeId",
                principalSchema: "Employee",
                principalTable: "SkillType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CallQueue_SkillType_SkillTypeId",
                schema: "Aws",
                table: "CallQueue");

            migrationBuilder.RenameColumn(
                name: "SkillTypeId",
                schema: "Aws",
                table: "CallQueue",
                newName: "SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_CallQueue_SkillTypeId",
                schema: "Aws",
                table: "CallQueue",
                newName: "IX_CallQueue_SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_CallQueue_Skills_SkillId",
                schema: "Aws",
                table: "CallQueue",
                column: "SkillId",
                principalSchema: "Employee",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
