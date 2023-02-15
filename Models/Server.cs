using System;
using System.Collections.Generic;

namespace DiscordClone.Models;

public partial class Server
{
    public int ServerId { get; set; }

    public string? Name { get; set; }

    public string? InviteCode { get; set; }

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<UserServer> UserServers { get; } = new List<UserServer>();
}
