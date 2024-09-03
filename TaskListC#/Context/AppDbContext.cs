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
    public DbSet<UserToDoTask> UserToDoTasks { get; set; }
    public DbSet<Invite> Invites { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Priority> Priorities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Friendship>()
          .HasOne(f => f.User)
          .WithMany(u => u.Friendships)
          .HasForeignKey(f => f.UserId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<Friendship>()
          .HasOne(f => f.Friend)
          .WithMany()
          .HasForeignKey(f => f.FriendId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<Invite>()
          .HasOne(i => i.Sender)
          .WithMany(u => u.SentInvites)
          .HasForeignKey(i => i.SenderId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<Invite>()
          .HasOne(i => i.Receiver)
          .WithMany(u => u.ReceivedInvites)
          .HasForeignKey(i => i.ReceiverId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<UserToDoTask>()
          .HasOne(ut => ut.User)
          .WithMany(u => u.UserToDoTasks)
          .HasForeignKey(ut => ut.UserId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<UserToDoTask>()
          .HasOne(ut => ut.ToDoTask)
          .WithMany(t => t.UserToDoTasks)
          .HasForeignKey(ut => ut.ToDoTaskId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<ToDoTask>()
        .HasOne(t => t.Priority)
        .WithMany(p => p.ToDoTasks)
        .HasForeignKey(t => t.PriorityId)
        .OnDelete(DeleteBehavior.NoAction);
    }
  }
}
