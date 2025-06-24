using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameLessonContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonContents_Files_FileId",
                table: "LessonContents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonContents",
                table: "LessonContents");

            migrationBuilder.RenameTable(
                name: "LessonContents",
                newName: "LessonSectionContents");

            migrationBuilder.RenameIndex(
                name: "IX_LessonContents_FileId",
                table: "LessonSectionContents",
                newName: "IX_LessonSectionContents_FileId");

            migrationBuilder.AlterColumn<string>(
                name: "content_type",
                table: "LessonSectionContents",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonSectionContents",
                table: "LessonSectionContents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSectionContents_Files_FileId",
                table: "LessonSectionContents",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonSectionContents_Files_FileId",
                table: "LessonSectionContents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonSectionContents",
                table: "LessonSectionContents");

            migrationBuilder.RenameTable(
                name: "LessonSectionContents",
                newName: "LessonContents");

            migrationBuilder.RenameIndex(
                name: "IX_LessonSectionContents_FileId",
                table: "LessonContents",
                newName: "IX_LessonContents_FileId");

            migrationBuilder.AlterColumn<string>(
                name: "content_type",
                table: "LessonContents",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonContents",
                table: "LessonContents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonContents_Files_FileId",
                table: "LessonContents",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
