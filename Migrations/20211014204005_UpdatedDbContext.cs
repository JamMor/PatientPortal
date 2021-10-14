using Microsoft.EntityFrameworkCore.Migrations;

namespace PatientPortal.Migrations
{
    public partial class UpdatedDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_Conversation_ConversationId",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_MessagingLink_MessagingLinkId",
                table: "ConversationParticipant");

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

            migrationBuilder.DropForeignKey(
                name: "FK_Unread_Message_MessageId",
                table: "Unread");

            migrationBuilder.DropForeignKey(
                name: "FK_Unread_MessagingLink_MessagingLinkId",
                table: "Unread");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Unread",
                table: "Unread");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessagingLink",
                table: "MessagingLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Message",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationParticipant",
                table: "ConversationParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation");

            migrationBuilder.RenameTable(
                name: "Unread",
                newName: "UnreadMessages");

            migrationBuilder.RenameTable(
                name: "MessagingLink",
                newName: "MessagingLinks");

            migrationBuilder.RenameTable(
                name: "Message",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "ConversationParticipant",
                newName: "ConversationParticipants");

            migrationBuilder.RenameTable(
                name: "Conversation",
                newName: "Conversations");

            migrationBuilder.RenameIndex(
                name: "IX_Unread_MessagingLinkId",
                table: "UnreadMessages",
                newName: "IX_UnreadMessages_MessagingLinkId");

            migrationBuilder.RenameIndex(
                name: "IX_Unread_MessageId",
                table: "UnreadMessages",
                newName: "IX_UnreadMessages_MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_Message_MessagingLinkId",
                table: "Messages",
                newName: "IX_Messages_MessagingLinkId");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ConversationId",
                table: "Messages",
                newName: "IX_Messages_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipant_MessagingLinkId",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_MessagingLinkId");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipant_ConversationId",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnreadMessages",
                table: "UnreadMessages",
                column: "UnreadId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessagingLinks",
                table: "MessagingLinks",
                column: "MessagingLinkId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants",
                column: "ConversationParticipantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Conversations_ConversationId",
                table: "ConversationParticipants",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_MessagingLinks_MessagingLinkId",
                table: "ConversationParticipants",
                column: "MessagingLinkId",
                principalTable: "MessagingLinks",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_MessagingLinks_MessagingLinkId",
                table: "Messages",
                column: "MessagingLinkId",
                principalTable: "MessagingLinks",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_MessagingLinks_MessagingLinkId1",
                table: "Patients",
                column: "MessagingLinkId1",
                principalTable: "MessagingLinks",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_MessagingLinks_MessagingLinkId1",
                table: "Staff",
                column: "MessagingLinkId1",
                principalTable: "MessagingLinks",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnreadMessages_Messages_MessageId",
                table: "UnreadMessages",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnreadMessages_MessagingLinks_MessagingLinkId",
                table: "UnreadMessages",
                column: "MessagingLinkId",
                principalTable: "MessagingLinks",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Conversations_ConversationId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_MessagingLinks_MessagingLinkId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_MessagingLinks_MessagingLinkId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_MessagingLinks_MessagingLinkId1",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_MessagingLinks_MessagingLinkId1",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_UnreadMessages_Messages_MessageId",
                table: "UnreadMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_UnreadMessages_MessagingLinks_MessagingLinkId",
                table: "UnreadMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnreadMessages",
                table: "UnreadMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessagingLinks",
                table: "MessagingLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants");

            migrationBuilder.RenameTable(
                name: "UnreadMessages",
                newName: "Unread");

            migrationBuilder.RenameTable(
                name: "MessagingLinks",
                newName: "MessagingLink");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "Message");

            migrationBuilder.RenameTable(
                name: "Conversations",
                newName: "Conversation");

            migrationBuilder.RenameTable(
                name: "ConversationParticipants",
                newName: "ConversationParticipant");

            migrationBuilder.RenameIndex(
                name: "IX_UnreadMessages_MessagingLinkId",
                table: "Unread",
                newName: "IX_Unread_MessagingLinkId");

            migrationBuilder.RenameIndex(
                name: "IX_UnreadMessages_MessageId",
                table: "Unread",
                newName: "IX_Unread_MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_MessagingLinkId",
                table: "Message",
                newName: "IX_Message_MessagingLinkId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ConversationId",
                table: "Message",
                newName: "IX_Message_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_MessagingLinkId",
                table: "ConversationParticipant",
                newName: "IX_ConversationParticipant_MessagingLinkId");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_ConversationId",
                table: "ConversationParticipant",
                newName: "IX_ConversationParticipant_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Unread",
                table: "Unread",
                column: "UnreadId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessagingLink",
                table: "MessagingLink",
                column: "MessagingLinkId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Message",
                table: "Message",
                column: "MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation",
                column: "ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationParticipant",
                table: "ConversationParticipant",
                column: "ConversationParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_Conversation_ConversationId",
                table: "ConversationParticipant",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_MessagingLink_MessagingLinkId",
                table: "ConversationParticipant",
                column: "MessagingLinkId",
                principalTable: "MessagingLink",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Unread_Message_MessageId",
                table: "Unread",
                column: "MessageId",
                principalTable: "Message",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Unread_MessagingLink_MessagingLinkId",
                table: "Unread",
                column: "MessagingLinkId",
                principalTable: "MessagingLink",
                principalColumn: "MessagingLinkId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
