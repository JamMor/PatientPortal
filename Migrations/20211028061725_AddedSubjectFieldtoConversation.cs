using Microsoft.EntityFrameworkCore.Migrations;

namespace PatientPortal.Migrations
{
    public partial class AddedSubjectFieldtoConversation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Conversations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Conversations");
        }
    }
}
