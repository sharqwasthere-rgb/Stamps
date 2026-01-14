using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Stamps.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Store> Stores { get; set; }
    public DbSet<StampCard> StampCards { get; set; }
    public DbSet<StampCardType> StampCardTypes { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<QRToken> QRTokens { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set audit fields
        var currentUserId = _httpContextAccessor?.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = currentUserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = currentUserId;
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Implement soft delete
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = currentUserId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure global query filter for soft deletes
        builder.Entity<Store>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StampCard>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StampCardType>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Transaction>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<QRToken>().HasQueryFilter(e => !e.IsDeleted);

        // Configure Store
        builder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasOne(e => e.Owner)
                .WithMany(u => u.OwnedStores)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure StampCardType
        builder.Entity<StampCardType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.StoreId);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasOne(e => e.Store)
                .WithMany()
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure StampCard
        builder.Entity<StampCard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.StoreId, e.UserId });
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasOne(e => e.Store)
                .WithMany(s => s.StampCards)
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User)
                .WithMany(u => u.StampCards)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CardType)
                .WithMany(ct => ct.StampCards)
                .HasForeignKey(e => e.CardTypeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Transaction
        builder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.StoreId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasOne(e => e.StampCard)
                .WithMany(sc => sc.Transactions)
                .HasForeignKey(e => e.StampCardId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Store)
                .WithMany(s => s.Transactions)
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.StoreOwner)
                .WithMany()
                .HasForeignKey(e => e.StoreOwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure QRToken
        builder.Entity<QRToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpiresAt);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasOne(e => e.User)
                .WithMany(u => u.QRTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.StampCard)
                .WithMany()
                .HasForeignKey(e => e.StampCardId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

