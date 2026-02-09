namespace LogBook.Web.Data.Entities;

public class LogEntry
{
    public int Id { get; set; }
    public int LogbookId { get; set; }
    public DateTime EntryDate { get; set; }
    public decimal? Duration { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Logbook Logbook { get; set; } = null!;
}
