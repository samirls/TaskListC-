using TaskListC_.Models;

namespace TaskListC_.ViewModel
{
  public class TaskWithUsersVM
  {
    public ToDoTask Task { get; set; }
    public List<User> Users { get; set; }
  }
}
