using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class skilltypefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Employees_EmployeesId",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillType_SkillTypeId1",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_EmployeesId",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_SkillTypeId1",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "EmployeesId",
                schema: "Employee",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "SkillTypeId1",
                schema: "Employee",
                table: "Skills");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeesId",
                schema: "Employee",
                table: "Skills",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SkillTypeId1",
                schema: "Employee",
                table: "Skills",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_EmployeesId",
                schema: "Employee",
                table: "Skills",
                column: "EmployeesId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SkillTypeId1",
                schema: "Employee",
                table: "Skills",
                column: "SkillTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Employees_EmployeesId",
                schema: "Employee",
                table: "Skills",
                column: "EmployeesId",
                principalSchema: "Employee",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillType_SkillTypeId1",
                schema: "Employee",
                table: "Skills",
                column: "SkillTypeId1",
                principalSchema: "Employee",
                principalTable: "SkillType",
                principalColumn: "Id");
        }
    }
}
