namespace DiscordClone.Models;

public class Message
{
    public int MessageId { get; set; }

    public int UserId { get; set; }

    public int ServerId { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public virtual Server Server { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}