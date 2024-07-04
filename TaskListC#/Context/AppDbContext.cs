using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskListC_.Models;

namespace TaskListC_.Context
{
  public class AppDbContext : IdentityDbContext<User>
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ToDoTask> ToDoTasks { get; set; }
  }
}
