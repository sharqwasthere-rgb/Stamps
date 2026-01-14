using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stamps.Web.Data;
using Stamps.Web.Services;
using System.Text.Json;

namespace Stamps.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StampCardsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IStampService _stampService;
    private readonly IQRCodeService _qrCodeService;
    private readonly ILogger<StampCardsController> _logger;

    public StampCardsController(
        ApplicationDbContext context, 
        IStampService stampService,
        IQRCodeService qrCodeService,
        ILogger<StampCardsController> logger)
    {
        _context = context;
        _stampService = stampService;
        _qrCodeService = qrCodeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all stamp cards for a user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserStampCards(string userId)
    {
        var cards = await _context.StampCards
            .Include(sc => sc.Store)
            .Where(sc => sc.UserId == userId && !sc.IsDeleted)
            .OrderByDescending(sc => sc.IsComplete)
            .ThenByDescending(sc => sc.UpdatedAt ?? sc.CreatedAt)
            .Select(sc => new StampCardDto
            {
                Id = sc.Id,
                StoreId = sc.StoreId,
                StoreName = sc.Store.Name,
                StoreAddress = sc.Store.Address ?? "",
                CurrentStamps = sc.CurrentStamps,
                RequiredStamps = sc.RequiredStamps,
                IsComplete = sc.IsComplete,
                LastUpdated = sc.UpdatedAt ?? sc.CreatedAt
            })
            .ToListAsync();

        return Ok(cards);
    }

    /// <summary>
    /// Store owner adds a stamp to customer's card by scanning their QR code
    /// </summary>
    [HttpPost("add-stamp")]
    public async Task<IActionResult> AddStamp([FromBody] AddStampRequest request)
    {
        _logger.LogInformation("Adding stamp for customer {CustomerId} at store {StoreId}", 
            request.CustomerId, request.StoreId);

        // Verify store owner
        var store = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == request.StoreId && s.OwnerId == request.StoreOwnerId && !s.IsDeleted);

        if (store == null)
        {
            return BadRequest(new { error = "Store not found or you don't own this store" });
        }

        // Verify customer exists
        var customer = await _context.Users.FindAsync(request.CustomerId);
        if (customer == null)
        {
            return BadRequest(new { error = "Customer not found" });
        }

        // Get card type info if provided
        StampCardType? cardType = null;
        if (request.CardTypeId.HasValue)
        {
            cardType = await _context.StampCardTypes.FindAsync(request.CardTypeId.Value);
        }

        // Find or create stamp card for this customer at this store (and card type)
        var stampCard = await _context.StampCards
            .FirstOrDefaultAsync(sc => sc.UserId == request.CustomerId 
                && sc.StoreId == request.StoreId 
                && sc.CardTypeId == request.CardTypeId
                && !sc.IsDeleted
                && !sc.IsComplete);

        if (stampCard == null)
        {
            // Create new stamp card
            var requiredStamps = cardType?.RequiredStamps ?? (request.RequiredStamps > 0 ? request.RequiredStamps : 10);
            stampCard = new StampCard
            {
                UserId = request.CustomerId,
                StoreId = request.StoreId,
                CardTypeId = request.CardTypeId,
                CardName = cardType?.Name ?? store.Name,
                CurrentStamps = 0,
                RequiredStamps = requiredStamps
            };
            _context.StampCards.Add(stampCard);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new stamp card {CardId} for customer {CustomerId} with type {CardTypeId}", 
                stampCard.Id, request.CustomerId, request.CardTypeId);
        }

        // Add stamp(s)
        var stampsToAdd = request.StampsToAdd > 0 ? request.StampsToAdd : 1;
        stampCard.CurrentStamps += stampsToAdd;

        // Record transaction
        var transaction = new Transaction
        {
            StampCardId = stampCard.Id,
            StoreId = request.StoreId,
            UserId = request.CustomerId,
            StoreOwnerId = request.StoreOwnerId,
            StampsAdded = stampsToAdd,
            Type = TransactionType.StampAdded
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Added {Stamps} stamp(s) to card {CardId}. Total: {Total}/{Required}", 
            stampsToAdd, stampCard.Id, stampCard.CurrentStamps, stampCard.RequiredStamps);

        return Ok(new AddStampResponse
        {
            Success = true,
            TransactionId = transaction.Id,
            StampCardId = stampCard.Id,
            StoreName = store.Name,
            CurrentStamps = stampCard.CurrentStamps,
            RequiredStamps = stampCard.RequiredStamps,
            IsComplete = stampCard.IsComplete,
            Message = stampCard.IsComplete 
                ? $"Carta completata! {stampCard.CurrentStamps}/{stampCard.RequiredStamps} timbri" 
                : $"Timbro aggiunto! {stampCard.CurrentStamps}/{stampCard.RequiredStamps} timbri"
        });
    }

    /// <summary>
    /// Customer polls for recent stamp updates
    /// </summary>
    [HttpGet("user/{userId}/recent")]
    public async Task<IActionResult> GetRecentUpdates(string userId, [FromQuery] DateTime? since)
    {
        var sinceTime = since ?? DateTime.UtcNow.AddMinutes(-5);

        var recentTransactions = await _context.Transactions
            .Include(t => t.Store)
            .Include(t => t.StampCard)
            .Where(t => t.UserId == userId && t.CreatedAt > sinceTime)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new RecentUpdateDto
            {
                TransactionId = t.Id,
                StampCardId = t.StampCardId,
                StoreName = t.Store.Name,
                StampsAdded = t.StampsAdded,
                StampsRedeemed = t.StampsRedeemed,
                Type = t.Type.ToString(),
                CurrentStamps = t.StampCard.CurrentStamps,
                RequiredStamps = t.StampCard.RequiredStamps,
                IsComplete = t.StampCard.IsComplete,
                Timestamp = t.CreatedAt
            })
            .ToListAsync();

        return Ok(recentTransactions);
    }

    /// <summary>
    /// Generate redemption QR code for a completed card
    /// </summary>
    [HttpGet("{cardId}/redemption-qr")]
    public async Task<IActionResult> GetRedemptionQR(int cardId, [FromQuery] string userId)
    {
        var card = await _context.StampCards
            .Include(sc => sc.Store)
            .FirstOrDefaultAsync(sc => sc.Id == cardId && sc.UserId == userId && !sc.IsDeleted);

        if (card == null)
        {
            return NotFound(new { error = "Card not found" });
        }

        if (!card.IsComplete)
        {
            return BadRequest(new { error = "Card is not complete yet" });
        }

        // Generate special redemption QR code
        var redemptionData = new
        {
            type = "redemption",
            cardId = card.Id,
            userId = userId,
            storeId = card.StoreId,
            timestamp = DateTime.UtcNow.ToString("o")
        };

        var qrData = JsonSerializer.Serialize(redemptionData);
        var qrImage = _qrCodeService.GenerateUserQRCodeImage(qrData);

        return Ok(new
        {
            cardId = card.Id,
            storeName = card.Store.Name,
            stampsToRedeem = card.RequiredStamps,
            qrData = qrData,
            qrImage = qrImage
        });
    }

    /// <summary>
    /// Store owner redeems a completed card
    /// </summary>
    [HttpPost("redeem")]
    public async Task<IActionResult> RedeemCard([FromBody] RedeemRequest request)
    {
        _logger.LogInformation("Redeeming card {CardId} for customer {CustomerId}", 
            request.CardId, request.CustomerId);

        var card = await _context.StampCards
            .Include(sc => sc.Store)
            .FirstOrDefaultAsync(sc => sc.Id == request.CardId && sc.UserId == request.CustomerId && !sc.IsDeleted);

        if (card == null)
        {
            return NotFound(new { error = "Card not found" });
        }

        if (card.Store.OwnerId != request.StoreOwnerId)
        {
            return BadRequest(new { error = "You don't own this store" });
        }

        if (!card.IsComplete)
        {
            return BadRequest(new { error = "Card is not complete" });
        }

        // Reset the card (or delete it based on business logic)
        var redeemedStamps = card.RequiredStamps;
        card.CurrentStamps = 0; // Reset for new cycle

        // Record transaction
        var transaction = new Transaction
        {
            StampCardId = card.Id,
            StoreId = card.StoreId,
            UserId = request.CustomerId,
            StoreOwnerId = request.StoreOwnerId,
            StampsRedeemed = redeemedStamps,
            Type = TransactionType.Redemption
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Redeemed {Stamps} stamps from card {CardId}", redeemedStamps, card.Id);

        return Ok(new
        {
            success = true,
            transactionId = transaction.Id,
            message = $"Premio riscattato! {redeemedStamps} timbri utilizzati",
            storeName = card.Store.Name
        });
    }
}

// DTOs
public class StampCardDto
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public string StoreName { get; set; } = "";
    public string StoreAddress { get; set; } = "";
    public int CurrentStamps { get; set; }
    public int RequiredStamps { get; set; }
    public bool IsComplete { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class AddStampRequest
{
    public string CustomerId { get; set; } = "";
    public string StoreOwnerId { get; set; } = "";
    public int StoreId { get; set; }
    public int? CardTypeId { get; set; }
    public int StampsToAdd { get; set; } = 1;
    public int RequiredStamps { get; set; } = 10;
}

public class AddStampResponse
{
    public bool Success { get; set; }
    public int TransactionId { get; set; }
    public int StampCardId { get; set; }
    public string StoreName { get; set; } = "";
    public int CurrentStamps { get; set; }
    public int RequiredStamps { get; set; }
    public bool IsComplete { get; set; }
    public string Message { get; set; } = "";
}

public class RecentUpdateDto
{
    public int TransactionId { get; set; }
    public int StampCardId { get; set; }
    public string StoreName { get; set; } = "";
    public int StampsAdded { get; set; }
    public int StampsRedeemed { get; set; }
    public string Type { get; set; } = "";
    public int CurrentStamps { get; set; }
    public int RequiredStamps { get; set; }
    public bool IsComplete { get; set; }
    public DateTime Timestamp { get; set; }
}

public class RedeemRequest
{
    public int CardId { get; set; }
    public string CustomerId { get; set; } = "";
    public string StoreOwnerId { get; set; } = "";
}

