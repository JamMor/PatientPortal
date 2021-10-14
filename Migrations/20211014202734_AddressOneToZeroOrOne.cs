using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PatientPortal.Migrations
{
    public partial class AddressOneToZeroOrOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Patients_PatientId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_PatientId",
                table: "Addresses");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Addresses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Patients_AddressId",
                table: "Addresses",
                column: "AddressId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Patients_AddressId",
                table: "Addresses");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Addresses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PatientId",
                table: "Addresses",
                column: "PatientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Patients_PatientId",
                table: "Addresses",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
