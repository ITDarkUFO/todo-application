using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class ChangePriorityandUserinToDoItemtointids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_todo-item_priority_PriorityId",
                table: "todo-item");

            migrationBuilder.DropForeignKey(
                name: "FK_todo-item_user_UserId",
                table: "todo-item");

            migrationBuilder.DropIndex(
                name: "IX_todo-item_PriorityId",
                table: "todo-item");

            migrationBuilder.DropIndex(
                name: "IX_todo-item_UserId",
                table: "todo-item");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "todo-item",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "PriorityId",
                table: "todo-item",
                newName: "Priority");

            migrationBuilder.AddColumn<int>(
                name: "PriorityNavigationId",
                table: "todo-item",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserNavigationId",
                table: "todo-item",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_todo-item_PriorityNavigationId",
                table: "todo-item",
                column: "PriorityNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_todo-item_UserNavigationId",
                table: "todo-item",
                column: "UserNavigationId");

            migrationBuilder.AddForeignKey(
                name: "FK_todo-item_priority_PriorityNavigationId",
                table: "todo-item",
                column: "PriorityNavigationId",
                principalTable: "priority",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_todo-item_user_UserNavigationId",
                table: "todo-item",
                column: "UserNavigationId",
                principalTable: "user",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_todo-item_priority_PriorityNavigationId",
                table: "todo-item");

            migrationBuilder.DropForeignKey(
                name: "FK_todo-item_user_UserNavigationId",
                table: "todo-item");

            migrationBuilder.DropIndex(
                name: "IX_todo-item_PriorityNavigationId",
                table: "todo-item");

            migrationBuilder.DropIndex(
                name: "IX_todo-item_UserNavigationId",
                table: "todo-item");

            migrationBuilder.DropColumn(
                name: "PriorityNavigationId",
                table: "todo-item");

            migrationBuilder.DropColumn(
                name: "UserNavigationId",
                table: "todo-item");

            migrationBuilder.RenameColumn(
                name: "User",
                table: "todo-item",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "todo-item",
                newName: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_todo-item_PriorityId",
                table: "todo-item",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_todo-item_UserId",
                table: "todo-item",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_todo-item_priority_PriorityId",
                table: "todo-item",
                column: "PriorityId",
                principalTable: "priority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_todo-item_user_UserId",
                table: "todo-item",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
