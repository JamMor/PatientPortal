using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PatientPortal.Migrations
{
    public partial class CommonMessagingIDAndModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_StafftoPatientConversation_S2PConversationS2PId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_StafftoStaffConversation_S2SConversationId",
                table: "Message");

            migrationBuilder.DropTable(
                name: "StaffStafftoPatientConversation");

            migrationBuilder.DropTable(
                name: "StaffStafftoStaffConversation");

            migrationBuilder.DropTable(
                name: "StafftoPatientConversation");

            migrationBuilder.DropTable(
                name: "StafftoStaffConversation");

            migrationBuilder.DropIndex(
                name: "IX_Message_S2PConversationS2PId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_S2SConversationId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "S2PConversationS2PId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "S2PId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "S2SConversationId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "S2SId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Message");

            migrationBuilder.AddColumn<int>(
                name: "MessagingLinkId",
                table: "Staff",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessagingLinkId1",
                table: "Staff",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessagingLinkId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessagingLinkId1",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessagingLinkId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    ConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WithPatient = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.ConversationId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MessagingLink",
                columns: table => new
                {
                    MessagingLinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagingLink", x => x.MessagingLinkId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConversationParticipant",
                columns: table => new
                {
                    ConversationParticipantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessagingLinkId = table.Column<int>(type: "int", nullable: false),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationParticipant", x => x.ConversationParticipantId);
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversation",
                        principalColumn: "ConversationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_MessagingLink_MessagingLinkId",
                        column: x => x.MessagingLinkId,
                        principalTable: "MessagingLink",
                        principalColumn: "MessagingLinkId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Unread",
                columns: table => new
                {
                    UnreadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessagingLinkId = table.Column<int>(type: "int", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unread", x => x.UnreadId);
                    table.ForeignKey(
                        name: "FK_Unread_Message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Unread_MessagingLink_MessagingLinkId",
                        column: x => x.MessagingLinkId,
                        principalTable: "MessagingLink",
                        principalColumn: "MessagingLinkId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_MessagingLinkId1",
                table: "Staff",
                column: "MessagingLinkId1");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MessagingLinkId1",
                table: "Patients",
                column: "MessagingLinkId1");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationId",
                table: "Message",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_MessagingLinkId",
                table: "Message",
                column: "MessagingLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipant_ConversationId",
                table: "ConversationParticipant",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipant_MessagingLinkId",
                table: "ConversationParticipant",
                column: "MessagingLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_Unread_MessageId",
                table: "Unread",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Unread_MessagingLinkId",
                table: "Unread",
                column: "MessagingLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Conversation_ConversationId",
                table: "Message",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_MessagingLink_MessagingLinkId",
                table: "Message",
                column: "MessagingLinkId",
                principalTable: "MessagingLink",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_MessagingLink_MessagingLinkId1",
                table: "Patients",
                column: "MessagingLinkId1",
                principalTable: "MessagingLink",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_MessagingLink_MessagingLinkId1",
                table: "Staff",
                column: "MessagingLinkId1",
                principalTable: "MessagingLink",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Conversation_ConversationId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_MessagingLink_MessagingLinkId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_MessagingLink_MessagingLinkId1",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_MessagingLink_MessagingLinkId1",
                table: "Staff");

            migrationBuilder.DropTable(
                name: "ConversationParticipant");

            migrationBuilder.DropTable(
                name: "Unread");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropTable(
                name: "MessagingLink");

            migrationBuilder.DropIndex(
                name: "IX_Staff_MessagingLinkId1",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MessagingLinkId1",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Message_ConversationId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_MessagingLinkId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "MessagingLinkId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "MessagingLinkId1",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "MessagingLinkId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MessagingLinkId1",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "MessagingLinkId",
                table: "Message");

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Message",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "S2PConversationS2PId",
                table: "Message",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "S2PId",
                table: "Message",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "S2SConversationId",
                table: "Message",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "S2SId",
                table: "Message",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Message",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StafftoPatientConversation",
                columns: table => new
                {
                    S2PId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MessagingPatient = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Message_StafftoPatientConversation_S2PConversationS2PId",
                table: "Message",
                column: "S2PConversationS2PId",
                principalTable: "StafftoPatientConversation",
                principalColumn: "S2PId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_StafftoStaffConversation_S2SConversationId",
                table: "Message",
                column: "S2SConversationId",
                principalTable: "StafftoStaffConversation",
                principalColumn: "S2SConversationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
