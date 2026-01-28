using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class passdown1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UiStAioLongTagStatusatus",
                schema: "Tools",
                table: "OstPassdown",
                newName: "AioLongTagStatusatus");

            migrationBuilder.AddColumn<int>(
                name: "CurrentAsa",
                schema: "Tools",
                table: "OstPassdown",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentAwsQueue",
                schema: "Tools",
                table: "OstPassdown",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAsa",
                schema: "Tools",
                table: "OstPassdown");

            migrationBuilder.DropColumn(
                name: "CurrentAwsQueue",
                schema: "Tools",
                table: "OstPassdown");

            migrationBuilder.RenameColumn(
                name: "AioLongTagStatusatus",
                schema: "Tools",
                table: "OstPassdown",
                newName: "UiStAioLongTagStatusatus");
        }
    }
}
