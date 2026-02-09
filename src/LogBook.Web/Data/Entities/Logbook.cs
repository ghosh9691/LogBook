namespace LogBook.Web.Data.Entities;

public class Logbook
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<LogEntry> Entries { get; set; } = new List<LogEntry>();
}
