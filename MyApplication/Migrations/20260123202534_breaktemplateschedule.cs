using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class breaktemplateschedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BreakTemplateId",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_BreakTemplateId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "BreakTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailedSchedule_BreakTemplates_BreakTemplateId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "BreakTemplateId",
                principalSchema: "Employee",
                principalTable: "BreakTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailedSchedule_BreakTemplates_BreakTemplateId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropIndex(
                name: "IX_DetailedSchedule_BreakTemplateId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropColumn(
                name: "BreakTemplateId",
                schema: "Employee",
                table: "DetailedSchedule");
        }
    }
}
