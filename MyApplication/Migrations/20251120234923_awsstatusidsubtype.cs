using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class awsstatusidsubtype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AwsStatusId",
                schema: "Employee",
                table: "ActivitySubType",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySubType_AwsStatusId",
                schema: "Employee",
                table: "ActivitySubType",
                column: "AwsStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivitySubType_Statuses_AwsStatusId",
                schema: "Employee",
                table: "ActivitySubType",
                column: "AwsStatusId",
                principalSchema: "Aws",
                principalTable: "Statuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivitySubType_Statuses_AwsStatusId",
                schema: "Employee",
                table: "ActivitySubType");

            migrationBuilder.DropIndex(
                name: "IX_ActivitySubType_AwsStatusId",
                schema: "Employee",
                table: "ActivitySubType");

            migrationBuilder.DropColumn(
                name: "AwsStatusId",
                schema: "Employee",
                table: "ActivitySubType");
        }
    }
}
