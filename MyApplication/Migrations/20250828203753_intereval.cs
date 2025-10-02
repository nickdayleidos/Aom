using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class Intereval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NaCurrentSvdPosture",
                table: "IntervalSummary");

            migrationBuilder.RenameColumn(
                name: "CurrentNNPICallsOffered",
                table: "IntervalSummary",
                newName: "CurrentNnpiCallsOffered");

            migrationBuilder.RenameColumn(
                name: "CurrentNNPICallsAnswered",
                table: "IntervalSummary",
                newName: "CurrentNnpiCallsAnswered");

            migrationBuilder.RenameColumn(
                name: "CurrentNNPIAsa",
                table: "IntervalSummary",
                newName: "CurrentNnpiASA");

            migrationBuilder.RenameColumn(
                name: "MtdVmMTTR",
                table: "IntervalSummary",
                newName: "BlSrmValidationFailOldest");

            migrationBuilder.RenameColumn(
                name: "MtdEmailMTTR",
                table: "IntervalSummary",
                newName: "BlSrmUaUsnManOldest");

            migrationBuilder.RenameColumn(
                name: "MobilityQuotesInQueue",
                table: "IntervalSummary",
                newName: "MtdVipCallsOffered");

            migrationBuilder.RenameColumn(
                name: "CurrentSiprVmOldest",
                table: "IntervalSummary",
                newName: "BlSrmUaSocManOldest");

            migrationBuilder.RenameColumn(
                name: "CurrentSiprVmCount",
                table: "IntervalSummary",
                newName: "MtdVipCallsAnswered");

            migrationBuilder.RenameColumn(
                name: "BlSrmUaOldest",
                table: "IntervalSummary",
                newName: "BlSrmUaAutoOldest");

            migrationBuilder.RenameColumn(
                name: "BlSrmUaCount",
                table: "IntervalSummary",
                newName: "MtdVipASA");

            migrationBuilder.RenameColumn(
                name: "BlSrmMsdOldest",
                table: "IntervalSummary",
                newName: "BlSrmEmailBuildoutsOldest");

            migrationBuilder.RenameColumn(
                name: "BlSrmMsdCount",
                table: "IntervalSummary",
                newName: "CurrentVipCallsOffered");

            migrationBuilder.RenameColumn(
                name: "BlRdmSiprCount",
                table: "IntervalSummary",
                newName: "CurrentVipCallsAnswered");

            migrationBuilder.RenameColumn(
                name: "BlRdmNnpiCount",
                table: "IntervalSummary",
                newName: "CurrentVipASA");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentUser",
                table: "IntervalSummary",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentEssCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlOcmNiprFatalCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BlOcmNiprFatalOldest",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlOcmNiprHoldCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BlOcmNiprHoldOldest",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlOcmNiprReadyCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BlOcmNiprReadyOldest",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlOcmSiprFatalCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BlOcmSiprFatalOldest",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlOcmSiprHoldCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BlOcmSiprHoldOldest",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlOcmSiprReadyCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BlOcmSiprReadyOldest",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BlRdmUsnEsdOldest",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BlRdmUsnOldest",
                table: "IntervalSummary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlSrmEmailBuildoutsCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlSrmUaAutoCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlSrmUaSocManCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlSrmUaUsnManCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlSrmValidationFailCount",
                table: "IntervalSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlOcmNiprFatalCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmNiprFatalOldest",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmNiprHoldCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmNiprHoldOldest",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmNiprReadyCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmNiprReadyOldest",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmSiprFatalCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmSiprFatalOldest",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmSiprHoldCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmSiprHoldOldest",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmSiprReadyCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlOcmSiprReadyOldest",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlRdmUsnEsdOldest",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlRdmUsnOldest",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlSrmEmailBuildoutsCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlSrmUaAutoCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlSrmUaSocManCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlSrmUaUsnManCount",
                table: "IntervalSummary");

            migrationBuilder.DropColumn(
                name: "BlSrmValidationFailCount",
                table: "IntervalSummary");

            migrationBuilder.RenameColumn(
                name: "CurrentNnpiCallsOffered",
                table: "IntervalSummary",
                newName: "CurrentNNPICallsOffered");

            migrationBuilder.RenameColumn(
                name: "CurrentNnpiCallsAnswered",
                table: "IntervalSummary",
                newName: "CurrentNNPICallsAnswered");

            migrationBuilder.RenameColumn(
                name: "CurrentNnpiASA",
                table: "IntervalSummary",
                newName: "CurrentNNPIAsa");

            migrationBuilder.RenameColumn(
                name: "MtdVipCallsOffered",
                table: "IntervalSummary",
                newName: "MobilityQuotesInQueue");

            migrationBuilder.RenameColumn(
                name: "MtdVipCallsAnswered",
                table: "IntervalSummary",
                newName: "CurrentSiprVmCount");

            migrationBuilder.RenameColumn(
                name: "MtdVipASA",
                table: "IntervalSummary",
                newName: "BlSrmUaCount");

            migrationBuilder.RenameColumn(
                name: "CurrentVipCallsOffered",
                table: "IntervalSummary",
                newName: "BlSrmMsdCount");

            migrationBuilder.RenameColumn(
                name: "CurrentVipCallsAnswered",
                table: "IntervalSummary",
                newName: "BlRdmSiprCount");

            migrationBuilder.RenameColumn(
                name: "CurrentVipASA",
                table: "IntervalSummary",
                newName: "BlRdmNnpiCount");

            migrationBuilder.RenameColumn(
                name: "BlSrmValidationFailOldest",
                table: "IntervalSummary",
                newName: "MtdVmMTTR");

            migrationBuilder.RenameColumn(
                name: "BlSrmUaUsnManOldest",
                table: "IntervalSummary",
                newName: "MtdEmailMTTR");

            migrationBuilder.RenameColumn(
                name: "BlSrmUaSocManOldest",
                table: "IntervalSummary",
                newName: "CurrentSiprVmOldest");

            migrationBuilder.RenameColumn(
                name: "BlSrmUaAutoOldest",
                table: "IntervalSummary",
                newName: "BlSrmUaOldest");

            migrationBuilder.RenameColumn(
                name: "BlSrmEmailBuildoutsOldest",
                table: "IntervalSummary",
                newName: "BlSrmMsdOldest");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentUser",
                table: "IntervalSummary",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CurrentEssCount",
                table: "IntervalSummary",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NaCurrentSvdPosture",
                table: "IntervalSummary",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
