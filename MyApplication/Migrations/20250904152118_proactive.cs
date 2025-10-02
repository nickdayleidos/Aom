using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class Proactive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proactive",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProactiveTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsnInjectionAnnouncement = table.Column<string>(type: "varchar(4000)", nullable: false),
                    UsnSiteAnnouncement = table.Column<string>(type: "varchar(4000)", nullable: false),
                    UsnStatusAnnouncement = table.Column<string>(type: "varchar(4000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proactive", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Proactive");
        }
    }
}
