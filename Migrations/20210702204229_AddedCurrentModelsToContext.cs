using Microsoft.EntityFrameworkCore.Migrations;

namespace PatientPortal.Migrations
{
    public partial class AddedCurrentModelsToContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthIssue_Patients_PatientId",
                table: "HealthIssue");

            migrationBuilder.DropForeignKey(
                name: "FK_TestHealthIssueAssociation_HealthIssue_HealthIssueId",
                table: "TestHealthIssueAssociation");

            migrationBuilder.DropForeignKey(
                name: "FK_TestHealthIssueAssociation_TestResults_TestResultId",
                table: "TestHealthIssueAssociation");

            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Patients_PatientId",
                table: "Visit");

            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Staff_StaffId",
                table: "Visit");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitHealthIssueAssociation_HealthIssue_HealthIssueId",
                table: "VisitHealthIssueAssociation");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitHealthIssueAssociation_Visit_VisitId",
                table: "VisitHealthIssueAssociation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitHealthIssueAssociation",
                table: "VisitHealthIssueAssociation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visit",
                table: "Visit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestHealthIssueAssociation",
                table: "TestHealthIssueAssociation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HealthIssue",
                table: "HealthIssue");

            migrationBuilder.RenameTable(
                name: "VisitHealthIssueAssociation",
                newName: "VisitHealthIssueAssociations");

            migrationBuilder.RenameTable(
                name: "Visit",
                newName: "Visits");

            migrationBuilder.RenameTable(
                name: "TestHealthIssueAssociation",
                newName: "TestHealthIssueAssociations");

            migrationBuilder.RenameTable(
                name: "HealthIssue",
                newName: "HealthIssues");

            migrationBuilder.RenameIndex(
                name: "IX_VisitHealthIssueAssociation_VisitId",
                table: "VisitHealthIssueAssociations",
                newName: "IX_VisitHealthIssueAssociations_VisitId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitHealthIssueAssociation_HealthIssueId",
                table: "VisitHealthIssueAssociations",
                newName: "IX_VisitHealthIssueAssociations_HealthIssueId");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_StaffId",
                table: "Visits",
                newName: "IX_Visits_StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_PatientId",
                table: "Visits",
                newName: "IX_Visits_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_TestHealthIssueAssociation_TestResultId",
                table: "TestHealthIssueAssociations",
                newName: "IX_TestHealthIssueAssociations_TestResultId");

            migrationBuilder.RenameIndex(
                name: "IX_TestHealthIssueAssociation_HealthIssueId",
                table: "TestHealthIssueAssociations",
                newName: "IX_TestHealthIssueAssociations_HealthIssueId");

            migrationBuilder.RenameIndex(
                name: "IX_HealthIssue_PatientId",
                table: "HealthIssues",
                newName: "IX_HealthIssues_PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitHealthIssueAssociations",
                table: "VisitHealthIssueAssociations",
                column: "VisitHealthIssueAssociationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visits",
                table: "Visits",
                column: "VisitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestHealthIssueAssociations",
                table: "TestHealthIssueAssociations",
                column: "TestHealthIssueAssociationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HealthIssues",
                table: "HealthIssues",
                column: "HealthIssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthIssues_Patients_PatientId",
                table: "HealthIssues",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestHealthIssueAssociations_HealthIssues_HealthIssueId",
                table: "TestHealthIssueAssociations",
                column: "HealthIssueId",
                principalTable: "HealthIssues",
                principalColumn: "HealthIssueId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestHealthIssueAssociations_TestResults_TestResultId",
                table: "TestHealthIssueAssociations",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "TestResultId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitHealthIssueAssociations_HealthIssues_HealthIssueId",
                table: "VisitHealthIssueAssociations",
                column: "HealthIssueId",
                principalTable: "HealthIssues",
                principalColumn: "HealthIssueId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitHealthIssueAssociations_Visits_VisitId",
                table: "VisitHealthIssueAssociations",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "VisitId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Patients_PatientId",
                table: "Visits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Staff_StaffId",
                table: "Visits",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthIssues_Patients_PatientId",
                table: "HealthIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_TestHealthIssueAssociations_HealthIssues_HealthIssueId",
                table: "TestHealthIssueAssociations");

            migrationBuilder.DropForeignKey(
                name: "FK_TestHealthIssueAssociations_TestResults_TestResultId",
                table: "TestHealthIssueAssociations");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitHealthIssueAssociations_HealthIssues_HealthIssueId",
                table: "VisitHealthIssueAssociations");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitHealthIssueAssociations_Visits_VisitId",
                table: "VisitHealthIssueAssociations");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Patients_PatientId",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Staff_StaffId",
                table: "Visits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visits",
                table: "Visits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitHealthIssueAssociations",
                table: "VisitHealthIssueAssociations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestHealthIssueAssociations",
                table: "TestHealthIssueAssociations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HealthIssues",
                table: "HealthIssues");

            migrationBuilder.RenameTable(
                name: "Visits",
                newName: "Visit");

            migrationBuilder.RenameTable(
                name: "VisitHealthIssueAssociations",
                newName: "VisitHealthIssueAssociation");

            migrationBuilder.RenameTable(
                name: "TestHealthIssueAssociations",
                newName: "TestHealthIssueAssociation");

            migrationBuilder.RenameTable(
                name: "HealthIssues",
                newName: "HealthIssue");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_StaffId",
                table: "Visit",
                newName: "IX_Visit_StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_PatientId",
                table: "Visit",
                newName: "IX_Visit_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitHealthIssueAssociations_VisitId",
                table: "VisitHealthIssueAssociation",
                newName: "IX_VisitHealthIssueAssociation_VisitId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitHealthIssueAssociations_HealthIssueId",
                table: "VisitHealthIssueAssociation",
                newName: "IX_VisitHealthIssueAssociation_HealthIssueId");

            migrationBuilder.RenameIndex(
                name: "IX_TestHealthIssueAssociations_TestResultId",
                table: "TestHealthIssueAssociation",
                newName: "IX_TestHealthIssueAssociation_TestResultId");

            migrationBuilder.RenameIndex(
                name: "IX_TestHealthIssueAssociations_HealthIssueId",
                table: "TestHealthIssueAssociation",
                newName: "IX_TestHealthIssueAssociation_HealthIssueId");

            migrationBuilder.RenameIndex(
                name: "IX_HealthIssues_PatientId",
                table: "HealthIssue",
                newName: "IX_HealthIssue_PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visit",
                table: "Visit",
                column: "VisitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitHealthIssueAssociation",
                table: "VisitHealthIssueAssociation",
                column: "VisitHealthIssueAssociationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestHealthIssueAssociation",
                table: "TestHealthIssueAssociation",
                column: "TestHealthIssueAssociationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HealthIssue",
                table: "HealthIssue",
                column: "HealthIssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthIssue_Patients_PatientId",
                table: "HealthIssue",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestHealthIssueAssociation_HealthIssue_HealthIssueId",
                table: "TestHealthIssueAssociation",
                column: "HealthIssueId",
                principalTable: "HealthIssue",
                principalColumn: "HealthIssueId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestHealthIssueAssociation_TestResults_TestResultId",
                table: "TestHealthIssueAssociation",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "TestResultId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Patients_PatientId",
                table: "Visit",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Staff_StaffId",
                table: "Visit",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitHealthIssueAssociation_HealthIssue_HealthIssueId",
                table: "VisitHealthIssueAssociation",
                column: "HealthIssueId",
                principalTable: "HealthIssue",
                principalColumn: "HealthIssueId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitHealthIssueAssociation_Visit_VisitId",
                table: "VisitHealthIssueAssociation",
                column: "VisitId",
                principalTable: "Visit",
                principalColumn: "VisitId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
