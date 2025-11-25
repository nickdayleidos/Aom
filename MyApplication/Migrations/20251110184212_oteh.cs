using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class oteh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOtExempt",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.AddColumn<int>(
                name: "OvertimeRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHistory_OvertimeRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "OvertimeRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeHistory_AcrRequest_OvertimeRequestId",
                schema: "Employee",
                table: "EmployeeHistory",
                column: "OvertimeRequestId",
                principalSchema: "Employee",
                principalTable: "AcrRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeHistory_AcrRequest_OvertimeRequestId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeHistory_EmployeeId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeHistory_OvertimeRequestId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.DropColumn(
                name: "OvertimeRequestId",
                schema: "Employee",
                table: "EmployeeHistory");

            migrationBuilder.AddColumn<bool>(
                name: "IsOtExempt",
                schema: "Employee",
                table: "AcrOrganization",
                type: "bit",
                nullable: true);
        }
    }
}
