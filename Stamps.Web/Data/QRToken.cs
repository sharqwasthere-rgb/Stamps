namespace Stamps.Web.Data;

public class QRToken : BaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public QRTokenType Type { get; set; }
    public int? StampCardId { get; set; } // For redemption tokens
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual StampCard? StampCard { get; set; }
    
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;
}

public enum QRTokenType
{
    AddStamps,
    Redemption
}

