using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class tools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
