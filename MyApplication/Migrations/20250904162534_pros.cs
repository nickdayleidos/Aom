using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class pros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Proactive");

            migrationBuilder.CreateTable(
                name: "ProactiveAnnouncements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProactiveTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsnInjectionAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsnSiteAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsnStatusAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProactiveAnnouncements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProactiveAnnouncements");

            migrationBuilder.CreateTable(
                name: "Proactive",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProactiveTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsnInjectionAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsnSiteAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsnStatusAnnouncement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proactive", x => x.Id);
                });
        }
    }
}
