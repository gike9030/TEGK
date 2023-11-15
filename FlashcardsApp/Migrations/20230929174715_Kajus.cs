using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class Kajus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                               
            migrationBuilder.CreateTable(
                name: "FlashcardViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlashcardCollectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlashcardViewModel_FlashcardCollection_FlashcardCollectionId",
                        column: x => x.FlashcardCollectionId,
                        principalTable: "FlashcardCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_FlashcardViewModel_FlashcardCollectionId",
                table: "FlashcardViewModel",
                column: "FlashcardCollectionId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlashcardViewModel");

        }
    }
}
