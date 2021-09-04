using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageNotes.Migrations
{
    public partial class Log2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_TypeId",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Logs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_TypeId",
                table: "Logs",
                newName: "IX_Logs_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Logs",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                newName: "IX_Logs_TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_TypeId",
                table: "Logs",
                column: "TypeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
