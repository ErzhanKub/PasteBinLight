using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                    Role = table.Column<int>(type: "int", nullable: false),
                    ConfirmationToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                columns: new[] { "Id", "ConfirmationToken", "Email", "Password", "Role", "Username" },
                values: new object[] { new Guid("8be5a326-69e8-432d-b0cb-8b3a1814b2ee"), "ConfirmToken", "string@mail.com", "b4923143305b8c19f5c5031c406be92b99ce221e00c795598356d9fc0fc117bc", 2, "SuperAdmin2077CP" });

            migrationBuilder.InsertData(
                table: "Postes",
                columns: new[] { "Id", "DateCreated", "DeadLine", "DisLikes", "IsPrivate", "Likes", "Title", "Url", "UserId" },
                values: new object[] { new Guid("ed6019a8-2713-41f7-b4da-c94e1c5d8886"), new DateTime(2023, 11, 4, 18, 54, 34, 612, DateTimeKind.Local).AddTicks(1492), new DateTime(2023, 12, 4, 18, 54, 34, 612, DateTimeKind.Local).AddTicks(1504), 13L, false, 183L, "My day", "https://www.youtube.com/", new Guid("8be5a326-69e8-432d-b0cb-8b3a1814b2ee") });

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
