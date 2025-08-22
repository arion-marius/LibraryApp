using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Readers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BooksBorrowed = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReaderBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReaderId = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    PickUpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnedDate = table.Column<DateTime>(type: "datetime2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReaderBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReaderBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReaderBooks_Readers_ReaderId",
                        column: x => x.ReaderId,
                        principalTable: "Readers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "Stock", "Title" },
                values: new object[,]
                {
                    { 1, "Mihail Sadoveanu", 99, "Baltagul" },
                    { 2, "Marin Preda", 99, "Moromeții" },
                    { 3, "George Călinescu", 99, "Enigma Otiliei" },
                    { 4, "Liviu Rebreanu", 99, "Ion" },
                    { 5, "Camil Petrescu", 99, "Ultima noapte de dragoste, întâia noapte de război" },
                    { 6, "Ion Creangă", 99, "Amintiri din copilărie" },
                    { 7, "Mateiu Caragiale", 99, "Craii de Curtea-Veche" },
                    { 8, "Liviu Rebreanu", 99, "Pădurea spânzuraților" },
                    { 9, "Mircea Eliade", 99, "Scrinul negru" },
                    { 10, "Marin Preda", 99, "Cel mai iubit dintre pământeni" }
                });

            migrationBuilder.InsertData(
                table: "Readers",
                columns: new[] { "Id", "BooksBorrowed", "Email", "Name" },
                values: new object[,]
                {
                    { 1, 0, "andrei@gmail.com", "Andrei Durac" },
                    { 2, 0, "maria@yahoo.com", "Maria Ignat" },
                    { 3, 0, "ion.calinescu01@hotmail.com", "Ion Calinescu" },
                    { 4, 0, "elena.lasconi@gmail.com", "Elena Lasconi" },
                    { 5, 0, "MazareMih@yahoo.com", "Mihai Mazare" },
                    { 6, 0, "anaxyz@gmail.com", "Ana Ungureanu" },
                    { 7, 0, "branzescu@gmail.com", "Cristian Branzescu" },
                    { 8, 0, "ioana001@gmail.com", "Ioana Dumitrascu" },
                    { 9, 0, "vlade999@yahoo.com", "Vlad Ene" },
                    { 10, 0, "agache.dianamxn@gmail.com", "Diana Agache" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReaderBooks_BookId",
                table: "ReaderBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReaderBooks_ReaderId",
                table: "ReaderBooks",
                column: "ReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Readers_Email",
                table: "Readers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReaderBooks");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Readers");
        }
    }
}
