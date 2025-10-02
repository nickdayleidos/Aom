using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class opera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProactiveAnnouncement",
                schema: "Tools",
                table: "ProactiveAnnouncement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiStatus",
                schema: "Tools",
                table: "OiStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiSeverity",
                schema: "Tools",
                table: "OiSeverity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiEventUpdate",
                schema: "Tools",
                table: "OiEventUpdate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiEvent",
                schema: "Tools",
                table: "OiEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiCategory",
                schema: "Tools",
                table: "OiCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IntervalSummary",
                schema: "Tools",
                table: "IntervalSummary");

            migrationBuilder.RenameTable(
                name: "EmailTemplates",
                schema: "Tools",
                newName: "EmailTemplates");

            migrationBuilder.RenameTable(
                name: "ProactiveAnnouncement",
                schema: "Tools",
                newName: "ProactiveAnnouncements");

            migrationBuilder.RenameTable(
                name: "OiStatus",
                schema: "Tools",
                newName: "OiStatuses");

            migrationBuilder.RenameTable(
                name: "OiSeverity",
                schema: "Tools",
                newName: "OiSeverities");

            migrationBuilder.RenameTable(
                name: "OiEventUpdate",
                schema: "Tools",
                newName: "OiEventUpdates");

            migrationBuilder.RenameTable(
                name: "OiEvent",
                schema: "Tools",
                newName: "OiEvents");

            migrationBuilder.RenameTable(
                name: "OiCategory",
                schema: "Tools",
                newName: "OiCategories");

            migrationBuilder.RenameTable(
                name: "IntervalSummary",
                schema: "Tools",
                newName: "IntervalSummaries");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentVmOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprUaifOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprGdaOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprEmailOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentEssOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentEmailOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentCustomerCareOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmValidationOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmValidationFailOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaUsnManOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaSocManOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaAutoOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmEmailBuildoutsOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmCxSatOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmAfuOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlRdmUsnOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlRdmUsnEsdOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprReadyOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprHoldOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprFatalOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprReadyOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprHoldOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprFatalOldest",
                table: "IntervalSummaries",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProactiveAnnouncements",
                table: "ProactiveAnnouncements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiStatuses",
                table: "OiStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiSeverities",
                table: "OiSeverities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiEventUpdates",
                table: "OiEventUpdates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiEvents",
                table: "OiEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiCategories",
                table: "OiCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IntervalSummaries",
                table: "IntervalSummaries",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OperaStatus",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperaType",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperaSubType",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperaTypeId = table.Column<int>(type: "int", nullable: false),
                    IsImpacting = table.Column<bool>(type: "bit", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaSubType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperaSubType_OperaType_OperaTypeId",
                        column: x => x.OperaTypeId,
                        principalSchema: "Employee",
                        principalTable: "OperaType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperaSubClass",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperaSubTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaSubClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperaSubClass_OperaSubType_OperaSubTypeId",
                        column: x => x.OperaSubTypeId,
                        principalSchema: "Employee",
                        principalTable: "OperaSubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperaRequest",
                schema: "Employee",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperaTypeId = table.Column<int>(type: "int", nullable: false),
                    OperaSubTypeId = table.Column<int>(type: "int", nullable: false),
                    OperaSubClassId = table.Column<int>(type: "int", nullable: true),
                    
                    SubmitterComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedWfm = table.Column<bool>(type: "bit", nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WfmComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperaRequest", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_OperaRequest_OperaSubClass_OperaSubClassId",
                        column: x => x.OperaSubClassId,
                        principalSchema: "Employee",
                        principalTable: "OperaSubClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperaRequest_OperaSubType_OperaSubTypeId",
                        column: x => x.OperaSubTypeId,
                        principalSchema: "Employee",
                        principalTable: "OperaSubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperaRequest_OperaType_OperaTypeId",
                        column: x => x.OperaTypeId,
                        principalSchema: "Employee",
                        principalTable: "OperaType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_EmployeeId_StartTime",
                schema: "Employee",
                table: "OperaRequest",
                columns: new[] { "EmployeeId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_OperaSubClassId",
                schema: "Employee",
                table: "OperaRequest",
                column: "OperaSubClassId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_OperaSubTypeId",
                schema: "Employee",
                table: "OperaRequest",
                column: "OperaSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_OperaTypeId",
                schema: "Employee",
                table: "OperaRequest",
                column: "OperaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaRequest_StartTime",
                schema: "Employee",
                table: "OperaRequest",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_OperaSubClass_OperaSubTypeId",
                schema: "Employee",
                table: "OperaSubClass",
                column: "OperaSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperaSubType_OperaTypeId",
                schema: "Employee",
                table: "OperaSubType",
                column: "OperaTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperaRequest",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "OperaStatus",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "OperaSubClass",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "OperaSubType",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "OperaType",
                schema: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProactiveAnnouncements",
                table: "ProactiveAnnouncements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiStatuses",
                table: "OiStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiSeverities",
                table: "OiSeverities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiEventUpdates",
                table: "OiEventUpdates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiEvents",
                table: "OiEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OiCategories",
                table: "OiCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IntervalSummaries",
                table: "IntervalSummaries");

            migrationBuilder.EnsureSchema(
                name: "Tools");

            migrationBuilder.RenameTable(
                name: "EmailTemplates",
                newName: "EmailTemplates",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "ProactiveAnnouncements",
                newName: "ProactiveAnnouncement",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "OiStatuses",
                newName: "OiStatus",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "OiSeverities",
                newName: "OiSeverity",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "OiEventUpdates",
                newName: "OiEventUpdate",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "OiEvents",
                newName: "OiEvent",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "OiCategories",
                newName: "OiCategory",
                newSchema: "Tools");

            migrationBuilder.RenameTable(
                name: "IntervalSummaries",
                newName: "IntervalSummary",
                newSchema: "Tools");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentVmOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprUaifOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprGdaOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentSiprEmailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentEssOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentEmailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentCustomerCareOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmValidationOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmValidationFailOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaUsnManOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaSocManOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmUaAutoOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmEmailBuildoutsOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmCxSatOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlSrmAfuOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlRdmUsnOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlRdmUsnEsdOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprReadyOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprHoldOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmSiprFatalOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprReadyOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprHoldOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlOcmNiprFatalOldest",
                schema: "Tools",
                table: "IntervalSummary",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProactiveAnnouncement",
                schema: "Tools",
                table: "ProactiveAnnouncement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiStatus",
                schema: "Tools",
                table: "OiStatus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiSeverity",
                schema: "Tools",
                table: "OiSeverity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiEventUpdate",
                schema: "Tools",
                table: "OiEventUpdate",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiEvent",
                schema: "Tools",
                table: "OiEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OiCategory",
                schema: "Tools",
                table: "OiCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IntervalSummary",
                schema: "Tools",
                table: "IntervalSummary",
                column: "Id");
        }
    }
}
