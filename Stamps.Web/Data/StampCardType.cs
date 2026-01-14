namespace Stamps.Web.Data;

public class StampCardType : BaseEntity
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RequiredStamps { get; set; } = 10;
    public string? RewardDescription { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public virtual Store Store { get; set; } = null!;
    public virtual ICollection<StampCard> StampCards { get; set; } = new List<StampCard>();
}

