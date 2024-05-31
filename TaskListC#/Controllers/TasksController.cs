using Microsoft.AspNetCore.Mvc;
using TaskListC_.Context;
using TaskListC_.Models;
using TaskListC_.ViewModel;

namespace TaskListC_.Controllers
{
  public class TasksController : Controller
  {
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
      _context = context;
    }
    public IActionResult Index()
    {
      var tasks = _context.ToDoTasks.ToList();
      return View(tasks);
    }
    public IActionResult CreateTask()
    {
      return View();
    }

    [HttpPost]
    public IActionResult CreateTask(CreateTaskVM task)
    {
      var taskToSave = new ToDoTask();
      taskToSave.TaskTitle = task.TaskTitle;
      taskToSave.TaskDescription = task.TaskDescription;

      _context.ToDoTasks.Add(taskToSave);
      _context.SaveChanges();
      TempData["success"] = "Task created successfully";
      return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteTask(int id)
    {
      var task = _context.ToDoTasks.FirstOrDefault(t => t.Id == id);
      if (task != null)
      {
        _context.ToDoTasks.Remove(task);
        _context.SaveChanges();
        TempData["success"] = "Task deleted successfully";
      }

      return RedirectToAction("Index");
    }

    public IActionResult EditTask(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      ToDoTask? task = _context.ToDoTasks.Find(id);

      if (task == null)
      {
        return NotFound();
      }

      return View(task);
    }

    [HttpPost]
    public IActionResult EditTask(ToDoTask obj)
    {
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
