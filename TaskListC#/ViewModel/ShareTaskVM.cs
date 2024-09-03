using TaskListC_.Models;

namespace TaskListC_.ViewModel
{
  public class ShareTaskVM
  {
    public ToDoTask Task { get; set; }
    public List<User> Friends { get; set; }
    public List<string> ExistingFriendIds { get; set; }
  }
}
