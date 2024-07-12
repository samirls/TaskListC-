using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskListC_.Models
{
  public class UserToDoTask
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [Required]
    public int ToDoTaskId { get; set; }

    [ForeignKey("ToDoTaskId")]
    public ToDoTask ToDoTask { get; set; }
  }
}
