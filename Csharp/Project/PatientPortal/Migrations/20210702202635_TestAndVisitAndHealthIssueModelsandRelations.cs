using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PatientPortal.Migrations
{
    public partial class TestAndVisitAndHealthIssueModelsandRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Visit");

            migrationBuilder.CreateTable(
                name: "HealthIssue",
                columns: table => new
                {
                    HealthIssueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShortDescription = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LongDescription = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthIssue", x => x.HealthIssueId);
                    table.ForeignKey(
                        name: "FK_HealthIssue_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TestHealthIssueAssociation",
                columns: table => new
                {
                    TestHealthIssueAssociationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TestResultId = table.Column<int>(type: "int", nullable: false),
                    HealthIssueId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestHealthIssueAssociation", x => x.TestHealthIssueAssociationId);
                    table.ForeignKey(
                        name: "FK_TestHealthIssueAssociation_HealthIssue_HealthIssueId",
                        column: x => x.HealthIssueId,
                        principalTable: "HealthIssue",
                        principalColumn: "HealthIssueId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestHealthIssueAssociation_TestResults_TestResultId",
                        column: x => x.TestResultId,
                        principalTable: "TestResults",
                        principalColumn: "TestResultId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VisitHealthIssueAssociation",
                columns: table => new
                {
                    VisitHealthIssueAssociationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VisitId = table.Column<int>(type: "int", nullable: false),
                    HealthIssueId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitHealthIssueAssociation", x => x.VisitHealthIssueAssociationId);
                    table.ForeignKey(
                        name: "FK_VisitHealthIssueAssociation_HealthIssue_HealthIssueId",
                        column: x => x.HealthIssueId,
                        principalTable: "HealthIssue",
                        principalColumn: "HealthIssueId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitHealthIssueAssociation_Visit_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visit",
                        principalColumn: "VisitId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HealthIssue_PatientId",
                table: "HealthIssue",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_TestHealthIssueAssociation_HealthIssueId",
                table: "TestHealthIssueAssociation",
                column: "HealthIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_TestHealthIssueAssociation_TestResultId",
                table: "TestHealthIssueAssociation",
                column: "TestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitHealthIssueAssociation_HealthIssueId",
                table: "VisitHealthIssueAssociation",
                column: "HealthIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitHealthIssueAssociation_VisitId",
                table: "VisitHealthIssueAssociation",
                column: "VisitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestHealthIssueAssociation");

            migrationBuilder.DropTable(
                name: "VisitHealthIssueAssociation");

            migrationBuilder.DropTable(
                name: "HealthIssue");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Visit",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
