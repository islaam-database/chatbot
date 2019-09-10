using Microsoft.EntityFrameworkCore.Migrations;

namespace Islaam.Migrations
{
    public partial class MainTitleSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MainTitleSource",
                table: "People",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainTitleSource",
                table: "People");
        }
    }
}
