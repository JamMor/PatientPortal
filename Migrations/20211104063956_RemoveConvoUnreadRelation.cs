using Microsoft.EntityFrameworkCore.Migrations;

namespace PatientPortal.Migrations
{
    public partial class RemoveConvoUnreadRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnreadMessages_Conversations_ConversationId",
                table: "UnreadMessages");

            migrationBuilder.DropIndex(
                name: "IX_UnreadMessages_ConversationId",
                table: "UnreadMessages");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "UnreadMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "UnreadMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UnreadMessages_ConversationId",
                table: "UnreadMessages",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnreadMessages_Conversations_ConversationId",
                table: "UnreadMessages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
