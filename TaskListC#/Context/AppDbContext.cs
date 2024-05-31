using Microsoft.EntityFrameworkCore;
using TaskListC_.Models;

namespace TaskListC_.Context
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ToDoTask> ToDoTasks { get; set; }
  }
}
