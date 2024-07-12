using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskListC_.Enums;

namespace TaskListC_.Models
{
  public class Invite
  {
    [Key]
    public int Id { get; set; }

    public InviteStatus InviteStatus { get; set; }

    [Required]
    public string SenderId { get; set; }
    [ForeignKey("SenderId")]
    public virtual User Sender { get; set; }

    [Required]
    public string ReceiverId { get; set; }
    [ForeignKey("ReceiverId")]
    public virtual User Receiver { get; set; }
  }
}
