using System.ComponentModel.DataAnnotations;

namespace TaskListC_.ViewModel
{
  public class CreateTaskVM
  {
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

    [Required(ErrorMessage = "You need to add a priority")]
    [Display(Name = "Priority")]
    public string TaskPriority { get; set; }
  }
}
