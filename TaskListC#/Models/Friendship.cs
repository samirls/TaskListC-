using System.ComponentModel.DataAnnotations;

namespace TaskListC_.Models
{
  public class Friendship
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }
    public User User { get; set; }

    [Required]
    public string FriendId { get; set; }
    public User Friend { get; set; }
  }
}
