using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TaskListC_.Context;
using TaskListC_.Models;
using TaskListC_.ViewModel;

namespace TaskListC_.Controllers
{
  public class FriendsController : Controller
  {

    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public FriendsController(AppDbContext context, UserManager<User> userManager)
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

      if (user == null)
      {
        return RedirectToAction("Login", "Account");
      }

      var friends = await _context.Friendships
        .Where(f => f.UserId == user.Id || f.FriendId == user.Id)
        .Include(f => f.User)
        .Include(f => f.Friend)
        .ToListAsync();

      var friendList = friends.Select(f => f.UserId == user.Id ? f.Friend : f.User).ToList();

      var viewModel = new UserFriendsViewModel
      {
        User = user,
        Friends = friendList
      };

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string FriendEmail)
    {
      var user = await _userManager.GetUserAsync(User);

      var friend = await _context.Users.SingleOrDefaultAsync(u => u.Email == FriendEmail);

      var friends = await _context.Friendships
        .Where(f => f.UserId == user.Id || f.FriendId == user.Id)
        .Include(f => f.User)
        .Include(f => f.Friend)
        .ToListAsync();

      var friendList = friends.Select(f => f.UserId == user.Id ? f.Friend : f.User).ToList();

      var viewModel = new UserFriendsViewModel
      {
        User = user,
        Friends = friendList
      };

      if (friend == null)
      {
        TempData["error"] = "Friend not found";
        return View(viewModel);
      }

      var isFriendAlreadyInvited = await _context.Invites.AnyAsync(
        u => u.SenderId == user.Id && u.ReceiverId == friend.Id);

      if (isFriendAlreadyInvited)
      {
        TempData["error"] = "Friend already invited";
        return View(viewModel);
      }

      var amIAlreadyInvitedByFriend = await _context.Invites.AnyAsync(
        u => u.SenderId == friend.Id && u.ReceiverId == user.Id);

      if (amIAlreadyInvitedByFriend)
      {
        TempData["error"] = "You were already invited by this friend, manage your friends";
        return View(viewModel);
      }

      if (FriendEmail == user.Email)
      {
        TempData["error"] = "Cannot add yourself";
        return View(viewModel);
      }

      Invite invite = new Invite();
      invite.InviteStatus = Enums.InviteStatus.Pending;
      invite.SenderId = user.Id;
      invite.ReceiverId = friend.Id;

      _context.Invites.Add(invite);
      await _context.SaveChangesAsync();

      TempData["success"] = "Friend invited";
      return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Invitations()
    {
      var user = await _userManager.GetUserAsync(User);

      var invitesReceived = await _context.Invites
        .Include(i => i.Sender)
        .Include(i => i.Receiver)
        .Where(i => i.ReceiverId == user.Id)
        .ToListAsync();

      var invitesSent = await _context.Invites
        .Include(i => i.Sender)
        .Include(i => i.Receiver)
        .Where(i => i.SenderId == user.Id)
        .ToListAsync();

      var viewModel = new InviteVM
      {
        InvitesReceived = invitesReceived,
        InvitesSent = invitesSent
      };

      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int InviteId)
    {

      var invite = await _context.Invites
        .Include(i => i.Sender)
        .Include(i => i.Receiver)
        .SingleOrDefaultAsync(i => i.Id == InviteId);

      if (invite == null)
      {
        TempData["error"] = "Invite not found";
        return RedirectToAction("Index");
      }

      invite.InviteStatus = Enums.InviteStatus.Read;

      _context.Invites.Update(invite);
      await _context.SaveChangesAsync();

      TempData["success"] = "Invite marked as read";

      return RedirectToAction("Invitations");
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsUnread(int InviteId)
    {

      var invite = await _context.Invites
        .Include(i => i.Sender)
        .Include(i => i.Receiver)
        .SingleOrDefaultAsync(i => i.Id == InviteId);

      if (invite == null)
      {
        TempData["error"] = "Invite not found";
        return RedirectToAction("Index");
      }

      invite.InviteStatus = Enums.InviteStatus.Pending;

      _context.Invites.Update(invite);
      await _context.SaveChangesAsync();

      TempData["success"] = "Invite marked as Unread";

      return RedirectToAction("Invitations");
    }

    [HttpPost]
    public async Task<IActionResult> AcceptInvite(int InviteId)
    {
      var invite = await _context.Invites
          .Include(i => i.Sender)
          .Include(i => i.Receiver)
          .SingleOrDefaultAsync(i => i.Id == InviteId);

      if (invite == null)
      {
        TempData["error"] = "Invite not found";
        return RedirectToAction("Invitations");
      }

      invite.InviteStatus = Enums.InviteStatus.Accepted;

      _context.Invites.Update(invite);

      var friendship = new Friendship
      {
        UserId = invite.SenderId,
        FriendId = invite.ReceiverId
      };

      _context.Friendships.Add(friendship);
      await _context.SaveChangesAsync();

      TempData["success"] = "Friend added successfully";

      return RedirectToAction("Invitations");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteInviteAndFriend(int InviteId)
    {
      var invite = await _context.Invites
          .Include(i => i.Sender)
          .Include(i => i.Receiver)
          .SingleOrDefaultAsync(i => i.Id == InviteId);

      if (invite == null)
      {
        TempData["error"] = "Invite not found";
        return RedirectToAction("Invitations");
      }

      var friendship = await _context.Friendships
        .Where(i => (i.UserId == invite.SenderId && i.FriendId == invite.ReceiverId) ||
                    (i.UserId == invite.ReceiverId && i.FriendId == invite.SenderId)
              )
        .FirstOrDefaultAsync();

      if (friendship != null)
      {
        _context.Friendships.Remove(friendship);
      }
      _context.Invites.Remove(invite);
      await _context.SaveChangesAsync();

      TempData["success"] = "Invite deleted successfully";

      return RedirectToAction("Invitations");
    }
  }
}
