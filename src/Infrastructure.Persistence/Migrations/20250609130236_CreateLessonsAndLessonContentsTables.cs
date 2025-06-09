using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateLessonsAndLessonContentsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonContents",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    content_type = table.Column<string>(
                        type: "nvarchar(13)",
                        maxLength: 13,
                        nullable: false
                    ),
                    Markdown_Type = table.Column<int>(type: "int", nullable: true),
                    Markdown_Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Markdown_Level = table.Column<int>(type: "int", nullable: true),
                    Markdown_Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonContents_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_LessonContents_FileId",
                table: "LessonContents",
                column: "FileId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "LessonContents");

            migrationBuilder.DropTable(name: "Lessons");
        }
    }
}
