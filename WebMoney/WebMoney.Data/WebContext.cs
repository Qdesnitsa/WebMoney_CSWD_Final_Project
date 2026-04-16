using Microsoft.EntityFrameworkCore;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data;

public class WebContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UsersProfiles { get; set; }
    public DbSet<IdentityDocument> IdentityDocuments { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<CardLimit> CardLimits { get; set; }
    public DbSet<CardUserProfile> CardUserProfiles { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public WebContext(DbContextOptions<WebContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardUserProfile>(cup =>
        {
            cup.ToTable("CardUserProfiles");
            cup.HasKey(k => new { k.CardId, k.UserProfileId });

            cup.HasOne(p => p.Card)
                .WithMany(p => p.CardUserProfiles)
                .HasForeignKey(k => k.CardId)
                .OnDelete(DeleteBehavior.SetNull);
        
            cup.HasOne(p => p.UserProfile)
                .WithMany(p => p.CardUserProfiles)
                .HasForeignKey(k => k.UserProfileId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

}