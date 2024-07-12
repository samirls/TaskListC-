using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskListC_.Models
{
  public class ToDoTask
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "You need to add a title")]
    [Display(Name = "Task Title")]
    [MinLength(5, ErrorMessage = "Your title is too short")]
    [MaxLength(80, ErrorMessage = "Your title must not exceed {1} letters")]
    public string TaskTitle { get; set; }

    [Required(ErrorMessage = "You need to add a description")]
    [Display(Name = "Task Description")]
    [MinLength(5, ErrorMessage = "Your description is too short")]
    [MaxLength(400, ErrorMessage = "Your description must not exceed {1} letters")]
    public string TaskDescription { get; set; }
    public DateTime? CreatedAt { get; set; }

    public DateTime? LastUpdate { get; set ; }

    public List<User> Users { get; set; }
    public string UpdatedByUserId { get; set; }

  }
}
