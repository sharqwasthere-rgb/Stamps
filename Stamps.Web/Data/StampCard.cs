namespace Stamps.Web.Data;

public class StampCard : BaseEntity
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? CardTypeId { get; set; }
    public string CardName { get; set; } = string.Empty;
    public int CurrentStamps { get; set; } = 0;
    public int RequiredStamps { get; set; } = 10;
    
    // Navigation properties
    public virtual Store Store { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual StampCardType? CardType { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    public bool IsComplete => CurrentStamps >= RequiredStamps;
}

