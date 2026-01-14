namespace Stamps.Web.Data;

public class Transaction : BaseEntity
{
    public int Id { get; set; }
    public int StampCardId { get; set; }
    public int StoreId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? StoreOwnerId { get; set; }
    public int StampsAdded { get; set; }
    public int StampsRedeemed { get; set; } = 0;
    public TransactionType Type { get; set; }
    
    // Navigation properties
    public virtual StampCard StampCard { get; set; } = null!;
    public virtual Store Store { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationUser? StoreOwner { get; set; }
}

public enum TransactionType
{
    StampAdded,
    Redemption
}

