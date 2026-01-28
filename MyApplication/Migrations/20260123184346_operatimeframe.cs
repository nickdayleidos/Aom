using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class operatimeframe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timeframe",
                schema: "Employee",
                table: "OperaRequest",
                newName: "TimeframeId");

            migrationBuilder.CreateTable(
                name: "OperaTimeframe",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaTimeframe", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_TimeframeId",
                schema: "Employee",
                table: "OperaRequest",
                column: "TimeframeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperaRequest_OperaTimeframe_TimeframeId",
                schema: "Employee",
                table: "OperaRequest",
                column: "TimeframeId",
                principalSchema: "Employee",
                principalTable: "OperaTimeframe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperaRequest_OperaTimeframe_TimeframeId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.DropTable(
                name: "OperaTimeframe",
                schema: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_OperaRequest_TimeframeId",
                schema: "Employee",
                table: "OperaRequest");

            migrationBuilder.RenameColumn(
                name: "TimeframeId",
                schema: "Employee",
                table: "OperaRequest",
                newName: "Timeframe");
        }
    }
}
