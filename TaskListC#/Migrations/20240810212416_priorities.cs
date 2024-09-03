using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskListC_.Migrations
{
    /// <inheritdoc />
    public partial class priorities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriorityId",
                table: "ToDoTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Priorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priorities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoTasks_PriorityId",
                table: "ToDoTasks",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoTasks_Priorities_PriorityId",
                table: "ToDoTasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoTasks_Priorities_PriorityId",
                table: "ToDoTasks");

            migrationBuilder.DropTable(
                name: "Priorities");

            migrationBuilder.DropIndex(
                name: "IX_ToDoTasks_PriorityId",
                table: "ToDoTasks");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "ToDoTasks");
        }
    }
}
