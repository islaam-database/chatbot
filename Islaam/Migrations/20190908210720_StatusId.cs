using Microsoft.EntityFrameworkCore.Migrations;

namespace Islaam.Migrations
{
    public partial class StatusId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Titles_Statuses_StatusRank",
                table: "Titles");

            migrationBuilder.RenameColumn(
                name: "StatusRank",
                table: "Titles",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Titles_StatusRank",
                table: "Titles",
                newName: "IX_Titles_StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Titles_Statuses_StatusId",
                table: "Titles",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Rank",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Titles_Statuses_StatusId",
                table: "Titles");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Titles",
                newName: "StatusRank");

            migrationBuilder.RenameIndex(
                name: "IX_Titles_StatusId",
                table: "Titles",
                newName: "IX_Titles_StatusRank");

            migrationBuilder.AddForeignKey(
                name: "FK_Titles_Statuses_StatusRank",
                table: "Titles",
                column: "StatusRank",
                principalTable: "Statuses",
                principalColumn: "Rank",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
