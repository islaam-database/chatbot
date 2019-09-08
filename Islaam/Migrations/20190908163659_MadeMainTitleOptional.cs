using Microsoft.EntityFrameworkCore.Migrations;

namespace Islaam.Migrations
{
    public partial class MadeMainTitleOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Praises_MainTitleId",
                table: "People");

            migrationBuilder.AlterColumn<int>(
                name: "MainTitleId",
                table: "People",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_People_Praises_MainTitleId",
                table: "People",
                column: "MainTitleId",
                principalTable: "Praises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Praises_MainTitleId",
                table: "People");

            migrationBuilder.AlterColumn<int>(
                name: "MainTitleId",
                table: "People",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Praises_MainTitleId",
                table: "People",
                column: "MainTitleId",
                principalTable: "Praises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
