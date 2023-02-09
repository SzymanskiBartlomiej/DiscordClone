using System;
using System.Collections.Generic;

namespace DiscordClone.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<Server> Servers { get; } = new List<Server>();
}
