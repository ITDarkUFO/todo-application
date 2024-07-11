using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddIdpkeytoPrioritytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_todo-item_priority_PriorityLevel",
                table: "todo-item");

            migrationBuilder.DropPrimaryKey(
                name: "priority_pkey",
                table: "priority");

            migrationBuilder.RenameColumn(
                name: "PriorityLevel",
                table: "todo-item",
                newName: "PriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_todo-item_PriorityLevel",
                table: "todo-item",
                newName: "IX_todo-item_PriorityId");

            migrationBuilder.AlterColumn<int>(
                name: "Level",
                table: "priority",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "priority",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "priority_pkey",
                table: "priority",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_todo-item_priority_PriorityId",
                table: "todo-item",
                column: "PriorityId",
                principalTable: "priority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_todo-item_priority_PriorityId",
                table: "todo-item");

            migrationBuilder.DropPrimaryKey(
                name: "priority_pkey",
                table: "priority");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "priority");

            migrationBuilder.RenameColumn(
                name: "PriorityId",
                table: "todo-item",
                newName: "PriorityLevel");

            migrationBuilder.RenameIndex(
                name: "IX_todo-item_PriorityId",
                table: "todo-item",
                newName: "IX_todo-item_PriorityLevel");

            migrationBuilder.AlterColumn<int>(
                name: "Level",
                table: "priority",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "priority_pkey",
                table: "priority",
                column: "Level");

            migrationBuilder.AddForeignKey(
                name: "FK_todo-item_priority_PriorityLevel",
                table: "todo-item",
                column: "PriorityLevel",
                principalTable: "priority",
                principalColumn: "Level",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
