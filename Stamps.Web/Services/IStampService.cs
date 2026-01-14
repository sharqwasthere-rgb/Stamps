using Stamps.Web.Data;

namespace Stamps.Web.Services;

public interface IStampService
{
    Task<bool> AddStampsAsync(string userId, int storeId, int stampCardId, int stampCount, string storeOwnerId);
    Task<bool> RedeemStampsAsync(string userId, int stampCardId, string storeOwnerId);
    Task<List<StampCard>> GetUserStampCardsAsync(string userId);
    Task<List<Transaction>> GetUserTransactionsAsync(string userId);
    Task<StoreStatistics> GetStoreStatisticsAsync(int storeId, string storeOwnerId);
}

public class StoreStatistics
{
    public int TotalCustomers { get; set; }
    public int TotalStampCards { get; set; }
    public int TotalStampsIssued { get; set; }
    public int TotalRedemptions { get; set; }
    public int ActiveCards { get; set; }
    public int CompletedCards { get; set; }
    public List<DailyStats> DailyStats { get; set; } = new();
}

public class DailyStats
{
    public DateTime Date { get; set; }
    public int StampsAdded { get; set; }
    public int Redemptions { get; set; }
    public int NewCustomers { get; set; }
}

