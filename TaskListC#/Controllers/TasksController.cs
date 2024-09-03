using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TaskListC_.Context;
using TaskListC_.Models;
using TaskListC_.ViewModel;

namespace TaskListC_.Controllers
{
  public class TasksController : Controller
  {
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public TasksController(AppDbContext context, UserManager<User> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      ViewBag.CurrentUrl = filterContext.HttpContext.Request.Path;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
      var user = await _userManager.GetUserAsync(User);

      var tasks = await _context.UserToDoTasks
          .Where(ut => ut.UserId == user.Id)
          .Select(ut => ut.ToDoTask)
          .ToListAsync();

      var taskWithUsersList = new List<TaskWithUsersVM>();

      foreach (var task in tasks)
      {
        var users = await _context.UserToDoTasks
            .Where(ut => ut.ToDoTaskId == task.Id)
            .Select(ut => ut.User)
            .ToListAsync();

        taskWithUsersList.Add(new TaskWithUsersVM
        {
          Task = task,
          Users = users
        });
      }

      return View(taskWithUsersList);
    }

    [HttpGet]
    public IActionResult CreateTask()
    {

      ViewBag.Priorities = new SelectList(_context.Priorities, "Id", "Level");

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(ToDoTask task)
    {
      var user = await _userManager.GetUserAsync(User);

      var taskToSave = new ToDoTask();
      taskToSave.TaskTitle = task.TaskTitle;
      taskToSave.TaskDescription = task.TaskDescription;
      taskToSave.PriorityId = task.PriorityId;
      taskToSave.CreatedAt = DateTime.Now;

      _context.ToDoTasks.Add(taskToSave);
      await _context.SaveChangesAsync();

      var userToDoTask = new UserToDoTask
      {
        UserId = user.Id,
        ToDoTaskId = taskToSave.Id
      };

      _context.UserToDoTasks.Add(userToDoTask);
      await _context.SaveChangesAsync();

      TempData["success"] = "Task created successfully";

      return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTask(int id)
    {
      var user = await _userManager.GetUserAsync(User);
      var task = await _context.ToDoTasks
        .Include(t => t.UserToDoTasks)
        .FirstOrDefaultAsync(t => t.Id == id);

      var usersOfTheTask = await _context.UserToDoTasks
        .Where(ut => ut.ToDoTaskId == id)
        .Select(ut => ut.UserId)
        .ToListAsync();

      if (usersOfTheTask[0] == user.Id)
      {
        _context.UserToDoTasks.RemoveRange(task.UserToDoTasks);
        _context.ToDoTasks.Remove(task);
        await _context.SaveChangesAsync();
        TempData["success"] = "Task deleted successfully";
      }
      else
      {
        TempData["error"] = "You cannot delete tasks created by other people";
      }

      return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> EditTask(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var user = await _userManager.GetUserAsync(User);

      ToDoTask task = await _context.ToDoTasks
        .Include(t => t.UserToDoTasks)
        .FirstOrDefaultAsync(t => t.Id == id);

      if (task == null)
      {
        return NotFound();
      }

      ViewBag.Priorities = new SelectList(_context.Priorities, "Id", "Level");

      return View(task);
    }

    [HttpPost]
    public async Task<IActionResult> EditTask(ToDoTask obj)
    {
      var user = await _userManager.GetUserAsync(User);

      var updateTask = await _context.ToDoTasks
        .Include(t => t.UserToDoTasks)
        .FirstOrDefaultAsync(t => t.Id == obj.Id);

      if (ModelState.IsValid)
      {
        updateTask.TaskTitle = obj.TaskTitle;
        updateTask.TaskDescription = obj.TaskDescription;
        updateTask.PriorityId = obj.PriorityId;
        updateTask.LastUpdate = DateTime.Now;
        updateTask.UpdatedByUserId = user.Name;

        _context.ToDoTasks.Update(updateTask);
        await _context.SaveChangesAsync();

        TempData["success"] = "Task updated successfully";

        return RedirectToAction("Index");
      }

      TempData["error"] = "Something went wrong";

      return View();
    }

    [HttpGet]
    public async Task<IActionResult> ShareTask(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var user = await _userManager.GetUserAsync(User);

      var usersOfTheTask = await _context.UserToDoTasks
        .Where(ut => ut.ToDoTaskId == id)
        .Select(ut => ut.UserId)
        .ToListAsync();

      if (usersOfTheTask[0] != user.Id)
      {
        TempData["error"] = "You cannot share a task that you did not create.";

        return RedirectToAction("Index");
      }

        ToDoTask task = await _context.ToDoTasks
        .Include(t => t.UserToDoTasks)
        .FirstOrDefaultAsync(t => t.Id == id);

      var friends = await _context.Friendships
        .Where(f => f.UserId == user.Id || f.FriendId == user.Id)
        .Include(f => f.User)
        .Include(f => f.Friend)
        .ToListAsync();

      var friendList = friends.Select(f => f.UserId == user.Id ? f.Friend : f.User).ToList();

      var existingFriendIds = await _context.UserToDoTasks
        .Where(ut => ut.ToDoTaskId == id)
        .Select(ut => ut.UserId)
        .ToListAsync();

      ShareTaskVM shareTaskVM = new ShareTaskVM();

      shareTaskVM.Task = task;
      shareTaskVM.Friends = friendList;
      shareTaskVM.ExistingFriendIds = existingFriendIds;


      return View(shareTaskVM);
    }

    [HttpPost]
    public async Task<IActionResult> ShareTaskSaveChanges(int? taskId, List<string> friendIds)
    {
      if (taskId == null || friendIds == null)
      {
        TempData["error"] = "Invalid task or friend selection";
        return RedirectToAction("Index");
      }

      var task = await _context.ToDoTasks
          .Include(t => t.UserToDoTasks)
          .FirstOrDefaultAsync(t => t.Id == taskId);

      var currentUser = await _userManager.GetUserAsync(User);

      var existingUserTasks = await _context.UserToDoTasks
        .Where(ut => ut.ToDoTaskId == taskId.Value)
        .ToListAsync();

      //Remove friends that are no longer selected
      foreach (var userTask in existingUserTasks)
      {
        if (userTask.UserId != currentUser.Id && !friendIds.Contains(userTask.UserId))
        {
          _context.UserToDoTasks.Remove(userTask);
        }
      }

      // Add new friends selected
      foreach (var friendId in friendIds)
      {
        if (!existingUserTasks.Any(ut => ut.UserId == friendId))
        {
          var newUserTask = new UserToDoTask
          {
            UserId = friendId,
            ToDoTaskId = taskId.Value
          };
          _context.UserToDoTasks.Add(newUserTask);
        }
      }

      await _context.SaveChangesAsync();

      TempData["success"] = "Task sharing updated successfully";

      return RedirectToAction("Index");
    }
  }
}
