using LogBook.Web.Data;
using LogBook.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogBook.Web.Services;

public class LogbookService
{
    private readonly ApplicationDbContext _context;

    public LogbookService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Logbook operations
    public async Task<List<Logbook>> GetAllLogbooksAsync()
    {
        return await _context.Logbooks
            .Include(l => l.Entries)
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Logbook?> GetLogbookAsync(int id)
    {
        return await _context.Logbooks
            .Include(l => l.Entries.OrderByDescending(e => e.EntryDate))
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Logbook> CreateLogbookAsync(string name, string? description, string? unit)
    {
        var logbook = new Logbook
        {
            Name = name,
            Description = description,
            Unit = unit,
            CreatedAt = DateTime.UtcNow
        };

        _context.Logbooks.Add(logbook);
        await _context.SaveChangesAsync();
        return logbook;
    }

    public async Task<bool> UpdateLogbookAsync(int id, string name, string? description, string? unit)
    {
        var logbook = await _context.Logbooks.FindAsync(id);
        if (logbook == null) return false;

        logbook.Name = name;
        logbook.Description = description;
        logbook.Unit = unit;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLogbookAsync(int id)
    {
        var logbook = await _context.Logbooks.FindAsync(id);
        if (logbook == null) return false;

        _context.Logbooks.Remove(logbook);
        await _context.SaveChangesAsync();
        return true;
    }

    // LogEntry operations
    public async Task<LogEntry> CreateEntryAsync(int logbookId, DateTime entryDate, decimal? duration, string? notes)
    {
        var entry = new LogEntry
        {
            LogbookId = logbookId,
            EntryDate = entryDate,
            Duration = duration,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.LogEntries.Add(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<bool> UpdateEntryAsync(int id, DateTime entryDate, decimal? duration, string? notes)
    {
        var entry = await _context.LogEntries.FindAsync(id);
        if (entry == null) return false;

        entry.EntryDate = entryDate;
        entry.Duration = duration;
        entry.Notes = notes;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEntryAsync(int id)
    {
        var entry = await _context.LogEntries.FindAsync(id);
        if (entry == null) return false;

        _context.LogEntries.Remove(entry);
        await _context.SaveChangesAsync();
        return true;
    }

    // Summary operations
    public decimal GetTotalDuration(Logbook logbook)
    {
        return logbook.Entries.Sum(e => e.Duration ?? 0);
    }

    public int GetEntryCount(Logbook logbook)
    {
        return logbook.Entries.Count;
    }
}
