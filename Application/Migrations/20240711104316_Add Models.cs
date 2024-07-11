using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "priority",
                columns: table => new
                {
                    Level = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("priority_pkey", x => x.Level);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "todo-item",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PriorityLevel = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("todo-item_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_todo-item_priority_PriorityLevel",
                        column: x => x.PriorityLevel,
                        principalTable: "priority",
                        principalColumn: "Level",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_todo-item_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_todo-item_PriorityLevel",
                table: "todo-item",
                column: "PriorityLevel");

            migrationBuilder.CreateIndex(
                name: "IX_todo-item_Title",
                table: "todo-item",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_todo-item_UserId",
                table: "todo-item",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_Name",
                table: "user",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "todo-item");

            migrationBuilder.DropTable(
                name: "priority");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
