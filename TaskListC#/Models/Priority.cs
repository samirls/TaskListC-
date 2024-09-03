using System.ComponentModel.DataAnnotations;

namespace TaskListC_.Models
{
  public class Priority
  {
    [Key]
    public int Id { get; set; }
    public string Level { get; set; }
    public ICollection<ToDoTask> ToDoTasks { get; set; }
  }
}
