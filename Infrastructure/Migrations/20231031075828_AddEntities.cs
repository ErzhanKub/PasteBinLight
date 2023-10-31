using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Postes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeadLine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Likes = table.Column<long>(type: "bigint", nullable: false),
                    DisLikes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Postes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("d2638162-bce7-4feb-a23a-c911c8f6e6a1"), "string@mail.com", "473287f8298dba7163a897908958f7c0eae733e25d2e027992ea2edc9bed2fa8", 1, "string" },
                    { new Guid("e4fefe5f-45f1-49ed-85c9-7c13f8b06d7c"), "erzhan@mail.com", "473287f8298dba7163a897908958f7c0eae733e25d2e027992ea2edc9bed2fa8", 2, "qwerty" }
                });

            migrationBuilder.InsertData(
                table: "Postes",
                columns: new[] { "Id", "DateCreated", "DeadLine", "DisLikes", "IsPrivate", "Likes", "Title", "Url", "UserId" },
                values: new object[] { new Guid("6a8274c8-b927-40c9-86a9-486dafe8f80f"), new DateTime(2023, 10, 31, 13, 58, 27, 970, DateTimeKind.Local).AddTicks(4953), new DateTime(2023, 11, 30, 13, 58, 27, 970, DateTimeKind.Local).AddTicks(4968), 13L, false, 183L, "My day", "https://www.youtube.com/", new Guid("d2638162-bce7-4feb-a23a-c911c8f6e6a1") });

            migrationBuilder.CreateIndex(
                name: "IX_Postes_UserId",
                table: "Postes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Postes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
