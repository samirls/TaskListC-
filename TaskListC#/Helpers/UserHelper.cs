using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TaskListC_.Models;
using System.Threading.Tasks;
using TaskListC_.Context;
using Microsoft.EntityFrameworkCore;
using TaskListC_.ViewModel;

namespace TaskListC_.Helpers
{
  public class UserHelper
  {
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;

    public UserHelper(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, AppDbContext context)
    {
      _userManager = userManager;
      _httpContextAccessor = httpContextAccessor;
      _context = context;
    }

    public async Task<string> GetUserNameAsync()
    {
      var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
      return user?.Name;
    }
    public async Task<List<Invite>> GetUserInvitesAsync()
    {
      var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

      if (user == null)
      {
        return new List<Invite>();
      }

      var invites = await _context.Invites
        .Include(i => i.Sender)
        .Include(i => i.Receiver)
        .Where(i => i.ReceiverId == user.Id && i.InviteStatus == Enums.InviteStatus.Pending)
        .ToListAsync();

      return invites;
    }
  }
}
