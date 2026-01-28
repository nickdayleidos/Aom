using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class scheduletype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScheduleTypeId",
                schema: "Employee",
                table: "DetailedSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScheduleType",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_ScheduleTypeId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "ScheduleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailedSchedule_ScheduleType_ScheduleTypeId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "ScheduleTypeId",
                principalSchema: "Employee",
                principalTable: "ScheduleType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailedSchedule_ScheduleType_ScheduleTypeId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropTable(
                name: "ScheduleType",
                schema: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_DetailedSchedule_ScheduleTypeId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropColumn(
                name: "ScheduleTypeId",
                schema: "Employee",
                table: "DetailedSchedule");
        }
    }
}
