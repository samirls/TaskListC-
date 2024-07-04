using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TaskListC_.Models;
using System.Threading.Tasks;

namespace TaskListC_.Helpers
{
  public class UserHelper
  {
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserHelper(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
      _userManager = userManager;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetUserNameAsync()
    {
      var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
      return user?.Name;
    }
  }
}
