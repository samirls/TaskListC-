using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
    public IActionResult CreateTask()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskVM task)
    {
      var user = await _userManager.GetUserAsync(User);

      var taskToSave = new ToDoTask();
      taskToSave.TaskTitle = task.TaskTitle;
      taskToSave.TaskDescription = task.TaskDescription;
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

      if (task != null)
      {
        _context.UserToDoTasks.RemoveRange(task.UserToDoTasks);
        _context.ToDoTasks.Remove(task);
        await _context.SaveChangesAsync();
        TempData["success"] = "Task deleted successfully";
      }
      else
      {
        TempData["error"] = "Something went wrong";
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
        updateTask.LastUpdate = DateTime.Now;

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

      ToDoTask task = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == id);

      var friends = await _context.Friendships
        .Where(f => f.UserId == user.Id || f.FriendId == user.Id)
        .Include(f => f.User)
        .Include(f => f.Friend)
        .ToListAsync();

      var friendList = friends.Select(f => f.UserId == user.Id ? f.Friend : f.User).ToList();

      ShareTaskVM shareTaskVM = new ShareTaskVM();

      shareTaskVM.Task = task;
      shareTaskVM.Friends = friendList;


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
          .FirstOrDefaultAsync(t => t.Id == taskId);

      var friends = await _context.Users
        .Where(u => friendIds.Contains(u.Id))
        .ToListAsync();

      if (friends.Count == 0)
      {
        TempData["error"] = "Invalid Friend selection";
      }

      var existingUserTasks = await _context.UserToDoTasks
        .Where(ut => ut.ToDoTaskId == taskId.Value && friendIds.Contains(ut.UserId))
        .ToListAsync();

      if (existingUserTasks.Count > 0)
      {
        TempData["error"] = "One or more selected friends already have access to the task.";
        return RedirectToAction("Index");
      }

      foreach (var friend in friends)
      {
        var userTask = new UserToDoTask
        {
          UserId = friend.Id,
          ToDoTaskId = taskId.Value
        };

        _context.UserToDoTasks.Add(userTask);
      }

      await _context.SaveChangesAsync();

      TempData["success"] = "Task shared successfully";

      return RedirectToAction("Index");
    }
  }
}
