using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskListC_.Migrations
{
    /// <inheritdoc />
    public partial class addTitleToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaskTitle",
                table: "ToDoTasks",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskTitle",
                table: "ToDoTasks");
        }
    }
}
