using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class acrfixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcrOrganization_AcrRequest_AcrRequestId1",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_AcrSchedule_AcrRequest_AcrRequestId1",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.DropIndex(
                name: "IX_AcrSchedule_AcrRequestId1",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.DropIndex(
                name: "IX_AcrOrganization_AcrRequestId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropIndex(
                name: "IX_AcrOrganization_AcrRequestId1",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.DropColumn(
                name: "AcrRequestId1",
                schema: "Employee",
                table: "AcrSchedule");

            migrationBuilder.DropColumn(
                name: "AcrRequestId1",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_AcrRequestId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "AcrRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AcrOrganization_AcrRequestId",
                schema: "Employee",
                table: "AcrOrganization");

            migrationBuilder.AddColumn<int>(
                name: "AcrRequestId1",
                schema: "Employee",
                table: "AcrSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcrRequestId1",
                schema: "Employee",
                table: "AcrOrganization",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcrSchedule_AcrRequestId1",
                schema: "Employee",
                table: "AcrSchedule",
                column: "AcrRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_AcrRequestId",
                schema: "Employee",
                table: "AcrOrganization",
                column: "AcrRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AcrOrganization_AcrRequestId1",
                schema: "Employee",
                table: "AcrOrganization",
                column: "AcrRequestId1",
                unique: true,
                filter: "[AcrRequestId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrOrganization_AcrRequest_AcrRequestId1",
                schema: "Employee",
                table: "AcrOrganization",
                column: "AcrRequestId1",
                principalSchema: "Employee",
                principalTable: "AcrRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcrSchedule_AcrRequest_AcrRequestId1",
                schema: "Employee",
                table: "AcrSchedule",
                column: "AcrRequestId1",
                principalSchema: "Employee",
                principalTable: "AcrRequest",
                principalColumn: "Id");
        }
    }
}
