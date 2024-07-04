using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskListC_.Models;
using TaskListC_.ViewModel;

namespace TaskListC_.Controllers
{
  public class AccountController : Controller
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      ViewBag.CurrentUrl = filterContext.HttpContext.Request.Path;
    }

    [HttpGet]
    public IActionResult Login()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
      if (!ModelState.IsValid)
      {
        return View(loginVM);
      }

      var user = await _userManager.FindByEmailAsync(loginVM.Email);

      if (user != null)
      {
        var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
        if (result.Succeeded)
        {
          TempData["success"] = "You are logged in!";
          return RedirectToAction("Index", "Tasks");
        }
      }

      ModelState.AddModelError("Error:", "Failed to Login, please try again");
      return View(loginVM);

    }

    [HttpGet]
    public IActionResult Register()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {

      if (ModelState.IsValid)
      {
        var user = new User { 
          Email = registerVM.Email,
          UserName = registerVM.Email,
          Name = registerVM.Name,
          Sex = registerVM.Sex,
          Color = registerVM.Color,
          Age = registerVM.Age
        };
        var result = await _userManager.CreateAsync(user, registerVM.Password);

        if(result.Succeeded)
        {
          //await _signInManager.SignInAsync(user, isPersistent: false);
          TempData["success"] = "Account Created";
          return RedirectToAction("Login", "Account");
        }
        else
        {
          this.ModelState.AddModelError("Register", "An error occured, please try again.");
        }
      }
      return View(registerVM);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
      HttpContext.Session.Clear();
      HttpContext.User = null;

      await _signInManager.SignOutAsync();

      return RedirectToAction("Index", "Home");
    }

  }
}
