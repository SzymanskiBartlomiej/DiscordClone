using System;
using System.Collections.Generic;

namespace DiscordClone.Models;

public partial class Server
{
    public int ServerId { get; set; }

    public int UserId { get; set; }

    public int ChatId { get; set; }

    public virtual User User { get; set; } = null!;
}
