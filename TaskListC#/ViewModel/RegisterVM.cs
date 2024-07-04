using System.ComponentModel.DataAnnotations;

namespace TaskListC_.ViewModel
{
  public class RegisterVM
  {
    [Required(ErrorMessage = "Write your Name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Write your Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Write your Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Inform your sex")]
    public string Sex { get; set; }

    [Required(ErrorMessage = "Inform a color")]
    public string Color { get; set; }

    [Required(ErrorMessage = "Inform your age")]
    public string Age { get; set; }

  }
}
