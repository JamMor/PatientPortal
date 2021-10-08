using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PatientPortal.Migrations
{
    public partial class MessagingModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StafftoPatientConversation",
                columns: table => new
                {
                    S2PId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessagingPatient = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StafftoPatientConversation", x => x.S2PId);
                    table.ForeignKey(
                        name: "FK_StafftoPatientConversation_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StafftoStaffConversation",
                columns: table => new
                {
                    S2SConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StafftoStaffConversation", x => x.S2SConversationId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StaffStafftoPatientConversation",
                columns: table => new
                {
                    MessagingStaffStaffId = table.Column<int>(type: "int", nullable: false),
                    PatientConversationsS2PId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffStafftoPatientConversation", x => new { x.MessagingStaffStaffId, x.PatientConversationsS2PId });
                    table.ForeignKey(
                        name: "FK_StaffStafftoPatientConversation_Staff_MessagingStaffStaffId",
                        column: x => x.MessagingStaffStaffId,
                        principalTable: "Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffStafftoPatientConversation_StafftoPatientConversation_P~",
                        column: x => x.PatientConversationsS2PId,
                        principalTable: "StafftoPatientConversation",
                        principalColumn: "S2PId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessageText = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    S2SId = table.Column<int>(type: "int", nullable: true),
                    S2PId = table.Column<int>(type: "int", nullable: true),
                    S2SConversationId = table.Column<int>(type: "int", nullable: true),
                    S2PConversationS2PId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Message_StafftoPatientConversation_S2PConversationS2PId",
                        column: x => x.S2PConversationS2PId,
                        principalTable: "StafftoPatientConversation",
                        principalColumn: "S2PId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Message_StafftoStaffConversation_S2SConversationId",
                        column: x => x.S2SConversationId,
                        principalTable: "StafftoStaffConversation",
                        principalColumn: "S2SConversationId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StaffStafftoStaffConversation",
                columns: table => new
                {
                    MessagingStaffStaffId = table.Column<int>(type: "int", nullable: false),
                    StaffConversationsS2SConversationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffStafftoStaffConversation", x => new { x.MessagingStaffStaffId, x.StaffConversationsS2SConversationId });
                    table.ForeignKey(
                        name: "FK_StaffStafftoStaffConversation_Staff_MessagingStaffStaffId",
                        column: x => x.MessagingStaffStaffId,
                        principalTable: "Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffStafftoStaffConversation_StafftoStaffConversation_Staff~",
                        column: x => x.StaffConversationsS2SConversationId,
                        principalTable: "StafftoStaffConversation",
                        principalColumn: "S2SConversationId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Message_S2PConversationS2PId",
                table: "Message",
                column: "S2PConversationS2PId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_S2SConversationId",
                table: "Message",
                column: "S2SConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffStafftoPatientConversation_PatientConversationsS2PId",
                table: "StaffStafftoPatientConversation",
                column: "PatientConversationsS2PId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffStafftoStaffConversation_StaffConversationsS2SConversat~",
                table: "StaffStafftoStaffConversation",
                column: "StaffConversationsS2SConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_StafftoPatientConversation_PatientId",
                table: "StafftoPatientConversation",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "StaffStafftoPatientConversation");

            migrationBuilder.DropTable(
                name: "StaffStafftoStaffConversation");

            migrationBuilder.DropTable(
                name: "StafftoPatientConversation");

            migrationBuilder.DropTable(
                name: "StafftoStaffConversation");
        }
    }
}
