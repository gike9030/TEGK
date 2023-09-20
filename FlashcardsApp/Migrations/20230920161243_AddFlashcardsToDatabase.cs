using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashcardsToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlashcardCollection",
                columns: table => new
                {
                    CollectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    Hearts = table.Column<int>(type: "int", nullable: false),
                    Haha = table.Column<int>(type: "int", nullable: false),
                    Like = table.Column<int>(type: "int", nullable: false),
                    Angry = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardCollection", x => x.CollectionId);
                });

            migrationBuilder.CreateTable(
                name: "FlashcardViewModel",
                columns: table => new
                {
                    FlashcardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlashcardCollectionCollectionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardViewModel", x => x.FlashcardId);
                    table.ForeignKey(
                        name: "FK_FlashcardViewModel_FlashcardCollection_FlashcardCollectionCollectionId",
                        column: x => x.FlashcardCollectionCollectionId,
                        principalTable: "FlashcardCollection",
                        principalColumn: "CollectionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardViewModel_FlashcardCollectionCollectionId",
                table: "FlashcardViewModel",
                column: "FlashcardCollectionCollectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlashcardViewModel");

            migrationBuilder.DropTable(
                name: "FlashcardCollection");
        }
    }
}
