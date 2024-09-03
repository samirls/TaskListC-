using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskListC_.Migrations
{
    /// <inheritdoc />
    public partial class priorities2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "Priorities",
                newName: "Level");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Priorities",
                newName: "Grade");
        }
    }
}
