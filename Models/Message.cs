using System;
using System.Collections.Generic;

namespace DiscordClone.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int UserId { get; set; }

    public int ChatId { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public virtual User User { get; set; } = null!;
}
