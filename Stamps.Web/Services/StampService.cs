using Microsoft.EntityFrameworkCore;
using Stamps.Web.Data;

namespace Stamps.Web.Services;

public class StampService : IStampService
{
    private readonly ApplicationDbContext _context;

    public StampService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddStampsAsync(string userId, int storeId, int stampCardId, int stampCount, string storeOwnerId)
    {
        var stampCard = await _context.StampCards
            .Include(sc => sc.Store)
            .FirstOrDefaultAsync(sc => sc.Id == stampCardId && sc.UserId == userId && sc.StoreId == storeId);

        if (stampCard == null || stampCard.Store.OwnerId != storeOwnerId)
        {
            return false;
        }

        stampCard.CurrentStamps += stampCount;

        var transaction = new Transaction
        {
            StampCardId = stampCardId,
            StoreId = storeId,
            UserId = userId,
            StoreOwnerId = storeOwnerId,
            StampsAdded = stampCount,
            Type = TransactionType.StampAdded
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RedeemStampsAsync(string userId, int stampCardId, string storeOwnerId)
    {
        var stampCard = await _context.StampCards
            .Include(sc => sc.Store)
            .FirstOrDefaultAsync(sc => sc.Id == stampCardId && sc.UserId == userId);

        if (stampCard == null || stampCard.Store.OwnerId != storeOwnerId)
        {
            return false;
        }

        if (stampCard.CurrentStamps < stampCard.RequiredStamps)
        {
            return false;
        }

        var stampsToRedeem = stampCard.RequiredStamps;
        stampCard.CurrentStamps -= stampsToRedeem;

        var transaction = new Transaction
        {
            StampCardId = stampCardId,
            StoreId = stampCard.StoreId,
            UserId = userId,
            StoreOwnerId = storeOwnerId,
            StampsRedeemed = stampsToRedeem,
            Type = TransactionType.Redemption
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<StampCard>> GetUserStampCardsAsync(string userId)
    {
        return await _context.StampCards
            .Include(sc => sc.Store)
            .Where(sc => sc.UserId == userId)
            .OrderByDescending(sc => sc.UpdatedAt ?? sc.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetUserTransactionsAsync(string userId)
    {
        return await _context.Transactions
            .Include(t => t.Store)
            .Include(t => t.StampCard)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<StoreStatistics> GetStoreStatisticsAsync(int storeId, string storeOwnerId)
    {
        var store = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId && s.OwnerId == storeOwnerId);

        if (store == null)
        {
            return new StoreStatistics();
        }

        var stampCards = await _context.StampCards
            .Where(sc => sc.StoreId == storeId)
            .ToListAsync();

        var transactions = await _context.Transactions
            .Where(t => t.StoreId == storeId)
            .ToListAsync();

        var stats = new StoreStatistics
        {
            TotalCustomers = stampCards.Select(sc => sc.UserId).Distinct().Count(),
            TotalStampCards = stampCards.Count,
            TotalStampsIssued = transactions.Where(t => t.Type == TransactionType.StampAdded).Sum(t => t.StampsAdded),
            TotalRedemptions = transactions.Count(t => t.Type == TransactionType.Redemption),
            ActiveCards = stampCards.Count(sc => sc.CurrentStamps > 0 && !sc.IsComplete),
            CompletedCards = stampCards.Count(sc => sc.IsComplete)
        };

        // Daily stats for last 30 days
        var last30Days = Enumerable.Range(0, 30)
            .Select(i => DateTime.UtcNow.Date.AddDays(-i))
            .ToList();

        stats.DailyStats = last30Days.Select(date => new DailyStats
        {
            Date = date,
            StampsAdded = transactions
                .Where(t => t.Type == TransactionType.StampAdded && t.CreatedAt.Date == date)
                .Sum(t => t.StampsAdded),
            Redemptions = transactions
                .Count(t => t.Type == TransactionType.Redemption && t.CreatedAt.Date == date),
            NewCustomers = stampCards
                .Count(sc => sc.CreatedAt.Date == date)
        }).ToList();

        return stats;
    }
}

