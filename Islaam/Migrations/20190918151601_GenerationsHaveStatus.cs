using Microsoft.EntityFrameworkCore.Migrations;

namespace Islaam.Migrations
{
    public partial class GenerationsHaveStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Generations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Generations_StatusId",
                table: "Generations",
                column: "StatusId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Generations_Statuses_StatusId",
                table: "Generations",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Rank",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Generations_Statuses_StatusId",
                table: "Generations");

            migrationBuilder.DropIndex(
                name: "IX_Generations_StatusId",
                table: "Generations");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Generations");
        }
    }
}
