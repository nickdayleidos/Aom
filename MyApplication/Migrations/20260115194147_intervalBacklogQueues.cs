using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class intervalBacklogQueues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Slr33VmMtdLos2",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Slr33VmMtdLos1",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Slr33EmMtdLos2",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Slr33EmMtdLos1",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NcisQueue",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NnpiQueue",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RdmNnpiQueue",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RdmSiprQueue",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiprQueue",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VipQueue",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NcisQueue",
                schema: "Tools",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "NnpiQueue",
                schema: "Tools",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "RdmNnpiQueue",
                schema: "Tools",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "RdmSiprQueue",
                schema: "Tools",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "SiprQueue",
                schema: "Tools",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "VipQueue",
                schema: "Tools",
                table: "IntervalSummary");

            migrationBuilder.AlterColumn<int>(
                name: "Slr33VmMtdLos2",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Slr33VmMtdLos1",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Slr33EmMtdLos2",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Slr33EmMtdLos1",
                schema: "Tools",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);
        }
    }
}
