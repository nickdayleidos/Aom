using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class passdown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OstPassdown",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewEdlId = table.Column<int>(type: "int", nullable: false),
                    PrevEdlId = table.Column<int>(type: "int", nullable: false),
                    PostedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AsaGoal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallbackProjectCount = table.Column<int>(type: "int", nullable: true),
                    CallbackProjectStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReskillTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReskillById = table.Column<int>(type: "int", nullable: true),
                    ProactiveTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProactiveById = table.Column<int>(type: "int", nullable: true),
                    HomeportTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HomeportById = table.Column<int>(type: "int", nullable: true),
                    SharepointTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SharepointById = table.Column<int>(type: "int", nullable: true),
                    OiStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UiStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UiStAioLongTagStatusatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtmStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NavyInboxJunkStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Los3Count = table.Column<int>(type: "int", nullable: true),
                    Los3Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenIdleCount = table.Column<int>(type: "int", nullable: true),
                    OpenIdleStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SvdRocCount = table.Column<int>(type: "int", nullable: true),
                    SvdRocStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SvdEtmCount = table.Column<int>(type: "int", nullable: true),
                    SvdEtmStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EdlComments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OstPassdown", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OstPassdown_Employees_HomeportById",
                        column: x => x.HomeportById,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OstPassdown_Employees_NewEdlId",
                        column: x => x.NewEdlId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OstPassdown_Employees_PrevEdlId",
                        column: x => x.PrevEdlId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OstPassdown_Employees_ProactiveById",
                        column: x => x.ProactiveById,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OstPassdown_Employees_ReskillById",
                        column: x => x.ReskillById,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OstPassdown_Employees_SharepointById",
                        column: x => x.SharepointById,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OstPassdown_HomeportById",
                schema: "Tools",
                table: "OstPassdown",
                column: "HomeportById");

            migrationBuilder.CreateIndex(
                name: "IX_OstPassdown_NewEdlId",
                schema: "Tools",
                table: "OstPassdown",
                column: "NewEdlId");

            migrationBuilder.CreateIndex(
                name: "IX_OstPassdown_PrevEdlId",
                schema: "Tools",
                table: "OstPassdown",
                column: "PrevEdlId");

            migrationBuilder.CreateIndex(
                name: "IX_OstPassdown_ProactiveById",
                schema: "Tools",
                table: "OstPassdown",
                column: "ProactiveById");

            migrationBuilder.CreateIndex(
                name: "IX_OstPassdown_ReskillById",
                schema: "Tools",
                table: "OstPassdown",
                column: "ReskillById");

            migrationBuilder.CreateIndex(
                name: "IX_OstPassdown_SharepointById",
                schema: "Tools",
                table: "OstPassdown",
                column: "SharepointById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OstPassdown",
                schema: "Tools");
        }
    }
}
