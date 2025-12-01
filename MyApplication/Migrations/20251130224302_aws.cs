using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class aws : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AwsId",
                schema: "Employee",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CallQueue",
                schema: "Aws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CallQueue_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "Employee",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Identifiers",
                schema: "Aws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwsUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identifiers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutingProfile",
                schema: "Aws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutingProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRoutingProfile",
                schema: "Aws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    WeekdayProfileId = table.Column<int>(type: "int", nullable: true),
                    WeekendProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRoutingProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeRoutingProfile_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeRoutingProfile_RoutingProfile_WeekdayProfileId",
                        column: x => x.WeekdayProfileId,
                        principalSchema: "Aws",
                        principalTable: "RoutingProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeRoutingProfile_RoutingProfile_WeekendProfileId",
                        column: x => x.WeekendProfileId,
                        principalSchema: "Aws",
                        principalTable: "RoutingProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutingProfileQueue",
                schema: "Aws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoutingProfileId = table.Column<int>(type: "int", nullable: false),
                    CallQueueId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Delay = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutingProfileQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutingProfileQueue_CallQueue_CallQueueId",
                        column: x => x.CallQueueId,
                        principalSchema: "Aws",
                        principalTable: "CallQueue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutingProfileQueue_RoutingProfile_RoutingProfileId",
                        column: x => x.RoutingProfileId,
                        principalSchema: "Aws",
                        principalTable: "RoutingProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AwsId",
                schema: "Employee",
                table: "Employees",
                column: "AwsId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailedSchedule_AwsStatusId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "AwsStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CallQueue_SkillId",
                schema: "Aws",
                table: "CallQueue",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoutingProfile_EmployeeId",
                schema: "Aws",
                table: "EmployeeRoutingProfile",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoutingProfile_WeekdayProfileId",
                schema: "Aws",
                table: "EmployeeRoutingProfile",
                column: "WeekdayProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoutingProfile_WeekendProfileId",
                schema: "Aws",
                table: "EmployeeRoutingProfile",
                column: "WeekendProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Identifiers_EmployeeId",
                schema: "Aws",
                table: "Identifiers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingProfileQueue_CallQueueId",
                schema: "Aws",
                table: "RoutingProfileQueue",
                column: "CallQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingProfileQueue_RoutingProfileId",
                schema: "Aws",
                table: "RoutingProfileQueue",
                column: "RoutingProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailedSchedule_Statuses_AwsStatusId",
                schema: "Employee",
                table: "DetailedSchedule",
                column: "AwsStatusId",
                principalSchema: "Aws",
                principalTable: "Statuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Identifiers_AwsId",
                schema: "Employee",
                table: "Employees",
                column: "AwsId",
                principalSchema: "Aws",
                principalTable: "Identifiers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailedSchedule_Statuses_AwsStatusId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Identifiers_AwsId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "EmployeeRoutingProfile",
                schema: "Aws");

            migrationBuilder.DropTable(
                name: "Identifiers",
                schema: "Aws");

            migrationBuilder.DropTable(
                name: "RoutingProfileQueue",
                schema: "Aws");

            migrationBuilder.DropTable(
                name: "CallQueue",
                schema: "Aws");

            migrationBuilder.DropTable(
                name: "RoutingProfile",
                schema: "Aws");

            migrationBuilder.DropIndex(
                name: "IX_Employees_AwsId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_DetailedSchedule_AwsStatusId",
                schema: "Employee",
                table: "DetailedSchedule");

            migrationBuilder.DropColumn(
                name: "AwsId",
                schema: "Employee",
                table: "Employees");
        }
    }
}
