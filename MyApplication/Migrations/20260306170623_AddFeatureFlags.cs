using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeatureFlag",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AllowedRoles = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureFlag", x => x.Id);
                });

            // Seed initial feature flags (all enabled by default)
            var flags = new (string Key, string Name, string Desc)[]
            {
                ("Module.Home",           "Home Dashboard",            "Call center dashboard and event calendar"),
                ("Module.ACR",            "ACR Requests",              "Allowance Change Request module"),
                ("Module.Opera",          "Opera Requests",            "Operational Request module"),
                ("Module.WFM",            "Workforce Management",      "WFM routing profiles and queue management"),
                ("Module.Training",       "Training & Certifications", "Skills lookup and certification tracking"),
                ("Module.Tools.Interval", "Interval Summary",          "Interval summary reports and email"),
                ("Module.Tools.OI",       "Operational Impact",        "Operational impact event tracking"),
                ("Module.Tools.Passdown", "OST Passdown",              "OST passdown dashboard"),
                ("Module.Tools.Proactive","Proactive Comms",           "Proactive communications tool"),
                ("Module.Admin",          "Admin Panel",               "Role management and debug tools")
            };

            foreach (var f in flags)
            {
                migrationBuilder.InsertData(
                    schema: "Tools",
                    table: "FeatureFlag",
                    columns: new[] { "Key", "DisplayName", "IsEnabled", "AllowedRoles", "Description" },
                    values: new object[] { f.Key, f.Name, true, null, f.Desc });
            }

            migrationBuilder.CreateIndex(
                name: "IX_FeatureFlag_Key",
                schema: "Tools",
                table: "FeatureFlag",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeatureFlag",
                schema: "Tools");
        }
    }
}

