using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MapeamentoUrbano.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UserNameNormalized = table.Column<string>(type: "text", nullable: false),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    UserPassword = table.Column<string>(type: "text", nullable: false),
                    FirstAccess = table.Column<bool>(type: "boolean", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Module = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Log_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Medals",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("043187e9-e471-43d8-8978-7fe73c83995d"), "Bronze" },
                    { new Guid("3c55e49f-fd3c-43ba-88d3-b990db7e5d01"), "Ouro" },
                    { new Guid("59c32bce-38ca-4d5d-a73f-76848fdf9f4d"), "Prata" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { new Guid("13124c76-5425-4494-8557-eef5db803bc8"), "Administrador" },
                    { new Guid("591b47b6-c0e8-43fd-8a38-986a2f897f61"), "User" },
                    { new Guid("75bad1fa-ea44-418e-a6a8-4eeca6167ec9"), "Master" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "FirstAccess", "RoleId", "UserEmail", "UserName", "UserNameNormalized", "UserPassword" },
                values: new object[] { new Guid("25b358c8-edcf-437d-81a0-08dbf7295ca5"), false, new Guid("75bad1fa-ea44-418e-a6a8-4eeca6167ec9"), "Master", "Master", "MASTER", "AITD5hKgy9KAb92qx88Wzid/4YxLHQJhkGDrwVt2rNlwjfyNc56vDg5NAUmuKeryRg==" });

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                table: "Log",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "Medals");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
