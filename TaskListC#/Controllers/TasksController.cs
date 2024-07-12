using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
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

      var tasks = await _context.ToDoTasks
        .Where(t => t.Users.Any(u => u.Id == user.Id))
        .ToListAsync();

      return View(tasks);
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
      taskToSave.Users = new List<User> { user };
      taskToSave.CreatedAt = DateTime.Now;

      _context.ToDoTasks.Add(taskToSave);
      await _context.SaveChangesAsync();
      TempData["success"] = "Task created successfully";
      return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTask(int id)
    {
      var user = await _userManager.GetUserAsync(User);
      var task = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == id);

      if (task != null)
      {
        _context.ToDoTasks.Remove(task);
        _context.SaveChanges();
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

      ToDoTask task = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == id);

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

      var updateTask = _context.ToDoTasks.FirstOrDefault(t => t.Id == obj.Id);

      updateTask.UpdatedByUserId = user.Id;
      updateTask.LastUpdate = DateTime.UtcNow;

      if (ModelState.IsValid)
      {
        _context.ToDoTasks.Update(obj);
        _context.SaveChanges();
        TempData["success"] = "Task updated successfully";
        return RedirectToAction("Index");
      }
      return View();
    }
  }
}
