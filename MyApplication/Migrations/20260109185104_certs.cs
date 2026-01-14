using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Migrations
{
    /// <inheritdoc />
    public partial class certs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificationVendors",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationVendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificationTypes",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false),
                    ShortName = table.Column<string>(type: "varchar(32)", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: true),
                    IsContinuingEducation = table.Column<int>(type: "int", nullable: true),
                    IatLevel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificationTypes_CertificationVendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "Employee",
                        principalTable: "CertificationVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certifications",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CertificationTypeId = table.Column<int>(type: "int", nullable: false),
                    CertificationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SerialNumber = table.Column<string>(type: "varchar(32)", nullable: false),
                    CeRegistrationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "varchar(32)", nullable: true),
                    FileName = table.Column<string>(type: "varchar(32)", nullable: false),
                    VerifiedBy = table.Column<string>(type: "varchar(32)", nullable: false),
                    Verified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certifications_CertificationTypes_CertificationTypeId",
                        column: x => x.CertificationTypeId,
                        principalSchema: "Employee",
                        principalTable: "CertificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certifications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Employee",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_CertificationTypeId",
                schema: "Employee",
                table: "Certifications",
                column: "CertificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_EmployeeId",
                schema: "Employee",
                table: "Certifications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationTypes_VendorId",
                schema: "Employee",
                table: "CertificationTypes",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certifications",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "CertificationTypes",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "CertificationVendors",
                schema: "Employee");
        }
    }
}
