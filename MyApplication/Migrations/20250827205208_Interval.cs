using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class Interval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntervalSummary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IntervalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IntervalStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    IntervalEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    CurrentUsnASA = table.Column<int>(type: "int", nullable: false),
                    CurrentUsnCallsOffered = table.Column<int>(type: "int", nullable: false),
                    CurrentUsnCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprASA = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprCallsOffered = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    CurrentNNPIAsa = table.Column<int>(type: "int", nullable: false),
                    CurrentNNPICallsOffered = table.Column<int>(type: "int", nullable: false),
                    CurrentNNPICallsAnswered = table.Column<int>(type: "int", nullable: false),
                    CurrentEmailCount = table.Column<int>(type: "int", nullable: false),
                    CurrentEmailOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentCustomerCareCount = table.Column<int>(type: "int", nullable: false),
                    CurrentCustomerCareOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentSiprEmailCount = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprEmailOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentVmCount = table.Column<int>(type: "int", nullable: false),
                    CurrentVmOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentEssCount = table.Column<int>(type: "int", nullable: true),
                    CurrentEssOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MtdUsnASA = table.Column<int>(type: "int", nullable: false),
                    MtdUsnCallsOffered = table.Column<int>(type: "int", nullable: false),
                    MtdUsnCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    MtdSiprASA = table.Column<int>(type: "int", nullable: false),
                    MtdSiprCallsOffered = table.Column<int>(type: "int", nullable: false),
                    MtdSiprCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    MtdNnpiASA = table.Column<int>(type: "int", nullable: false),
                    MtdNnpiCallsOffered = table.Column<int>(type: "int", nullable: false),
                    MtdNnpiCallsAnswered = table.Column<int>(type: "int", nullable: false),
                    MtdEmailMTTR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MtdVmMTTR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BlSrmValidationCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmValidationOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BlSrmUaCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmUaOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BlSrmMsdCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmMsdOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BlSrmAfuCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmAfuOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BlSrmCxSatCount = table.Column<int>(type: "int", nullable: false),
                    BlSrmCxSatOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BlRdmUsnCount = table.Column<int>(type: "int", nullable: false),
                    BlRdmNnpiCount = table.Column<int>(type: "int", nullable: false),
                    BlRdmSiprCount = table.Column<int>(type: "int", nullable: false),
                    BlRdmUsnEsdCount = table.Column<int>(type: "int", nullable: false),
                    NaTodaysFocusArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaMajorCirImpact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaImpactingEvents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaHpsmStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaCurrentSvdPosture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaManagementNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentSiprGdaSpreadsheets = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprGdaOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentSiprUaifCount = table.Column<int>(type: "int", nullable: false),
                    CurrentSiprUaifOldest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MobilityQuotesInQueue = table.Column<int>(type: "int", nullable: false),
                    Slr33EmMtdLos1 = table.Column<int>(type: "int", nullable: false),
                    Slr33EmMtdLos2 = table.Column<int>(type: "int", nullable: false),
                    Slr33VmMtdLos1 = table.Column<int>(type: "int", nullable: false),
                    Slr33VmMtdLos2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntervalSummary", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntervalSummary");
        }
    }
}
