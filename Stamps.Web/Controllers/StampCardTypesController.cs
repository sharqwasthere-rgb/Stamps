using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stamps.Web.Data;

namespace Stamps.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StampCardTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StampCardTypesController> _logger;

    public StampCardTypesController(ApplicationDbContext context, ILogger<StampCardTypesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/stampcardtypes/store/{storeId}
    [HttpGet("store/{storeId}")]
    public async Task<IActionResult> GetByStore(int storeId)
    {
        var cardTypes = await _context.StampCardTypes
            .Where(ct => ct.StoreId == storeId && ct.IsActive)
            .Select(ct => new
            {
                ct.Id,
                ct.Name,
                ct.Description,
                ct.RequiredStamps,
                ct.RewardDescription,
                ct.IsActive
            })
            .ToListAsync();

        return Ok(cardTypes);
    }

    // GET: api/stampcardtypes/owner/{ownerId}
    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> GetByOwner(string ownerId)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == ownerId);
        if (store == null)
            return NotFound(new { error = "Store not found" });

        var cardTypes = await _context.StampCardTypes
            .Where(ct => ct.StoreId == store.Id && ct.IsActive)
            .Select(ct => new
            {
                ct.Id,
                ct.Name,
                ct.Description,
                ct.RequiredStamps,
                ct.RewardDescription,
                ct.IsActive
            })
            .ToListAsync();

        return Ok(cardTypes);
    }

    // POST: api/stampcardtypes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardTypeRequest request)
    {
        _logger.LogInformation("Creating card type. OwnerId: {OwnerId}, Name: {Name}", request.OwnerId, request.Name);
        
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == request.OwnerId);
        if (store == null)
        {
            _logger.LogWarning("Store not found for owner {OwnerId}", request.OwnerId);
            return BadRequest(new { error = "Store not found for this owner", ownerId = request.OwnerId });
        }

        var cardType = new StampCardType
        {
            StoreId = store.Id,
            Name = request.Name,
            Description = request.Description,
            RequiredStamps = request.RequiredStamps,
            RewardDescription = request.RewardDescription,
            IsActive = true
        };

        _context.StampCardTypes.Add(cardType);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created card type {CardTypeName} for store {StoreId}", cardType.Name, store.Id);

        return Ok(new
        {
            success = true,
            cardType = new
            {
                cardType.Id,
                cardType.Name,
                cardType.Description,
                cardType.RequiredStamps,
                cardType.RewardDescription
            }
        });
    }

    // PUT: api/stampcardtypes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCardTypeRequest request)
    {
        var cardType = await _context.StampCardTypes.FindAsync(id);
        if (cardType == null)
            return NotFound(new { error = "Card type not found" });

        cardType.Name = request.Name ?? cardType.Name;
        cardType.Description = request.Description ?? cardType.Description;
        cardType.RequiredStamps = request.RequiredStamps ?? cardType.RequiredStamps;
        cardType.RewardDescription = request.RewardDescription ?? cardType.RewardDescription;
        cardType.IsActive = request.IsActive ?? cardType.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    // DELETE: api/stampcardtypes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cardType = await _context.StampCardTypes.FindAsync(id);
        if (cardType == null)
            return NotFound(new { error = "Card type not found" });

        cardType.IsActive = false;
        await _context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    public class CreateCardTypeRequest
    {
        [System.Text.Json.Serialization.JsonPropertyName("ownerId")]
        public string OwnerId { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("requiredStamps")]
        public int RequiredStamps { get; set; } = 10;
        
        [System.Text.Json.Serialization.JsonPropertyName("rewardDescription")]
        public string? RewardDescription { get; set; }
    }

    public class UpdateCardTypeRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? RequiredStamps { get; set; }
        public string? RewardDescription { get; set; }
        public bool? IsActive { get; set; }
    }
}

