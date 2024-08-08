using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskListC_.Migrations
{
    /// <inheritdoc />
    public partial class userToDoTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ToDoTasks_ToDoTaskId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ToDoTaskId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ToDoTaskId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UserToDoTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToDoTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToDoTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToDoTasks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserToDoTasks_ToDoTasks_ToDoTaskId",
                        column: x => x.ToDoTaskId,
                        principalTable: "ToDoTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToDoTasks_ToDoTaskId",
                table: "UserToDoTasks",
                column: "ToDoTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToDoTasks_UserId",
                table: "UserToDoTasks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToDoTasks");

            migrationBuilder.AddColumn<int>(
                name: "ToDoTaskId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ToDoTaskId",
                table: "AspNetUsers",
                column: "ToDoTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ToDoTasks_ToDoTaskId",
                table: "AspNetUsers",
                column: "ToDoTaskId",
                principalTable: "ToDoTasks",
                principalColumn: "Id");
        }
    }
}
