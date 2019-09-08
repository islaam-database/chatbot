using Microsoft.EntityFrameworkCore.Migrations;

namespace Islaam.Migrations
{
    public partial class MadeIntsOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Generations_GenerationId",
                table: "People");

            migrationBuilder.DropForeignKey(
                name: "FK_Praises_Titles_TitleId",
                table: "Praises");

            migrationBuilder.DropForeignKey(
                name: "FK_Praises_Topics_TopicId",
                table: "Praises");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Subjects_ParentSubjectId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherStudents_Subjects_SubjectId",
                table: "TeacherStudents");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TeacherStudents",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ParentSubjectId",
                table: "Subjects",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Praises",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "TitleId",
                table: "Praises",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "TaqreedId",
                table: "People",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "GenerationId",
                table: "People",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Generations",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Generations_GenerationId",
                table: "People",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Praises_Titles_TitleId",
                table: "Praises",
                column: "TitleId",
                principalTable: "Titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Praises_Topics_TopicId",
                table: "Praises",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Subjects_ParentSubjectId",
                table: "Subjects",
                column: "ParentSubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherStudents_Subjects_SubjectId",
                table: "TeacherStudents",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Generations_GenerationId",
                table: "People");

            migrationBuilder.DropForeignKey(
                name: "FK_Praises_Titles_TitleId",
                table: "Praises");

            migrationBuilder.DropForeignKey(
                name: "FK_Praises_Topics_TopicId",
                table: "Praises");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Subjects_ParentSubjectId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherStudents_Subjects_SubjectId",
                table: "TeacherStudents");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "TeacherStudents",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParentSubjectId",
                table: "Subjects",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Praises",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TitleId",
                table: "Praises",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TaqreedId",
                table: "People",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GenerationId",
                table: "People",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Generations",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_People_Generations_GenerationId",
                table: "People",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Praises_Titles_TitleId",
                table: "Praises",
                column: "TitleId",
                principalTable: "Titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Praises_Topics_TopicId",
                table: "Praises",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Subjects_ParentSubjectId",
                table: "Subjects",
                column: "ParentSubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherStudents_Subjects_SubjectId",
                table: "TeacherStudents",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
