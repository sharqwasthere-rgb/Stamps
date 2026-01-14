using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stamps.Web.Data;

namespace Stamps.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StoresController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetStores([FromQuery] double? lat, [FromQuery] double? lng)
    {
        var stores = await _context.Stores
            .Where(s => !s.IsDeleted)
            .Select(s => new
            {
                s.Id,
                s.Name,
                Address = $"{s.Address}, {s.City}",
                s.Latitude,
                s.Longitude,
                Distance = "0.5", // TODO: Calculate actual distance
                StampsRequired = 10 // Default
            })
            .ToListAsync();

        // If no stores in DB, return sample data
        if (stores.Count == 0)
        {
            return Ok(new[]
            {
                new { Id = 1, Name = "Caffè Roma", Address = "Via Roma 45, Milano", Latitude = 45.4642, Longitude = 9.1900, Distance = "0.3", StampsRequired = 10 },
                new { Id = 2, Name = "Pizzeria Napoli", Address = "Corso Italia 12, Milano", Latitude = 45.4654, Longitude = 9.1859, Distance = "0.5", StampsRequired = 8 },
                new { Id = 3, Name = "Gelateria Dolce", Address = "Piazza Duomo 7, Milano", Latitude = 45.4641, Longitude = 9.1919, Distance = "0.8", StampsRequired = 6 },
                new { Id = 4, Name = "Bar Sport", Address = "Via Montenapoleone 23, Milano", Latitude = 45.4685, Longitude = 9.1954, Distance = "1.2", StampsRequired = 10 },
                new { Id = 5, Name = "Pasticceria Milano", Address = "Via Torino 89, Milano", Latitude = 45.4612, Longitude = 9.1832, Distance = "1.5", StampsRequired = 12 }
            });
        }

        return Ok(stores);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStore(int id)
    {
        var store = await _context.Stores
            .Where(s => s.Id == id && !s.IsDeleted)
            .FirstOrDefaultAsync();

        if (store == null)
            return NotFound();

        return Ok(store);
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> GetStoreByOwner(string ownerId)
    {
        var store = await _context.Stores
            .Where(s => s.OwnerId == ownerId && !s.IsDeleted)
            .Select(s => new { s.Id, s.Name, s.Address, s.City })
            .FirstOrDefaultAsync();

        if (store == null)
        {
            // Return a default store for testing
            return Ok(new { Id = 1, Name = "Il tuo Negozio", Address = "", City = "" });
        }

        return Ok(store);
    }

    [HttpGet("owner/{ownerId}/stats")]
    public async Task<IActionResult> GetStoreStats(string ownerId)
    {
        var store = await _context.Stores
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId && !s.IsDeleted);

        if (store == null)
        {
            return Ok(new StoreStatsDto());
        }

        var today = DateTime.UtcNow.Date;

        var stampCards = await _context.StampCards
            .Where(sc => sc.StoreId == store.Id && !sc.IsDeleted)
            .ToListAsync();

        var transactions = await _context.Transactions
            .Where(t => t.StoreId == store.Id && !t.IsDeleted)
            .ToListAsync();

        var stats = new StoreStatsDto
        {
            StoreId = store.Id,
            StoreName = store.Name,
            TotalCustomers = stampCards.Select(sc => sc.UserId).Distinct().Count(),
            TodayStamps = transactions
                .Where(t => t.Type == TransactionType.StampAdded && t.CreatedAt.Date == today)
                .Sum(t => t.StampsAdded),
            ActiveCards = stampCards.Count(sc => sc.CurrentStamps > 0 && !sc.IsComplete),
            TotalRedemptions = transactions.Count(t => t.Type == TransactionType.Redemption),
            TotalStampsIssued = transactions
                .Where(t => t.Type == TransactionType.StampAdded)
                .Sum(t => t.StampsAdded),
            CompletedCards = stampCards.Count(sc => sc.IsComplete)
        };

        return Ok(stats);
    }
}

public class StoreStatsDto
{
    public int StoreId { get; set; }
    public string StoreName { get; set; } = "";
    public int TotalCustomers { get; set; }
    public int TodayStamps { get; set; }
    public int ActiveCards { get; set; }
    public int TotalRedemptions { get; set; }
    public int TotalStampsIssued { get; set; }
    public int CompletedCards { get; set; }
}

