using System.ComponentModel.DataAnnotations;

namespace TaskListC_.ViewModel
{
  public class LoginVM
  {

    [Required(ErrorMessage = "Write your Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Write your Password")]
    [DataType (DataType.Password)]
    public string Password { get; set; }

  }
}
