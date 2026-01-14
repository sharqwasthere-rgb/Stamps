using Microsoft.AspNetCore.Identity;

namespace Stamps.Web.Data;

public class ApplicationUser : IdentityUser
{
    public UserType UserType { get; set; }
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Store> OwnedStores { get; set; } = new List<Store>();
    public virtual ICollection<StampCard> StampCards { get; set; } = new List<StampCard>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<QRToken> QRTokens { get; set; } = new List<QRToken>();
}

public enum UserType
{
    Client,
    StoreOwner
}

