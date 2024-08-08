using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskListC_.Models
{
  public class User : IdentityUser
  {
    [Required(ErrorMessage = "Write your Name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Write your Email")]
    public override string Email { get; set; }

    [Required(ErrorMessage = "Inform your sex")]
    public string Sex { get; set; }

    [Required(ErrorMessage = "Inform a color")]
    public string Color { get; set; }

    [Required(ErrorMessage = "Inform your age")]
    public string Age { get; set; }

    public ICollection<Invite> SentInvites { get; set; }
    public ICollection<Invite> ReceivedInvites { get; set; }

    public List<Friendship> Friendships { get; set; }

    public ICollection<UserToDoTask> UserToDoTasks { get; set; }
  }
}
