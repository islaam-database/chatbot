using Microsoft.EntityFrameworkCore.Migrations;

namespace Islaam.Migrations
{
    public partial class TopicSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Topics",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentTopicId",
                table: "Topics",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_ParentTopicId",
                table: "Topics",
                column: "ParentTopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Topics_ParentTopicId",
                table: "Topics",
                column: "ParentTopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Topics_ParentTopicId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_ParentTopicId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "ParentTopicId",
                table: "Topics");
        }
    }
}
