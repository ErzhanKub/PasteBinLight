using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test1 : Migration
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
                values: new object[] { new Guid("323c8e8b-8e44-47bf-89ce-e8af73c8007d"), "ConfirmToken", "string@mail.com", "b4923143305b8c19f5c5031c406be92b99ce221e00c795598356d9fc0fc117bc", 2, "SuperAdmin2077CP" });

            migrationBuilder.InsertData(
                table: "Postes",
                columns: new[] { "Id", "DateCreated", "DeadLine", "DisLikes", "IsPrivate", "Likes", "Title", "Url", "UserId" },
                values: new object[] { new Guid("90c5b3b2-8a8d-44cf-ab9b-306dd828c465"), new DateTime(2023, 11, 7, 20, 44, 38, 91, DateTimeKind.Local).AddTicks(7317), new DateTime(2023, 12, 7, 20, 44, 38, 91, DateTimeKind.Local).AddTicks(7327), 13L, false, 183L, "My day", "https://www.youtube.com/", new Guid("323c8e8b-8e44-47bf-89ce-e8af73c8007d") });

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
