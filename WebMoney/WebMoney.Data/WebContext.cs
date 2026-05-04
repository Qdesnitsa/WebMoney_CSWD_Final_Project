using Microsoft.EntityFrameworkCore;
using WebMoney.Data.Entities;

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
    public DbSet<Counterparty> Counterparties { get; set; }

    public WebContext(DbContextOptions<WebContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role)
                .HasConversion<string>()
                .HasMaxLength(32);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("users_profiles");
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IdentityDocument>(entity =>
        {
            entity.ToTable("identity_documents");
            entity.HasOne(d => d.UserProfile)
                .WithOne(p => p.IdentityDocument)
                .HasForeignKey<UserProfile>(p => p.IdentityDocumentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.ToTable("cards");
            entity.Property(e => e.CurrencyCode)
                .HasConversion<string>()
                .HasMaxLength(16);
            entity.Property(e => e.CardStatus)
                .HasConversion<string>()
                .HasMaxLength(32);
        });

        modelBuilder.Entity<CardLimit>(entity => { entity.ToTable("card_limits"); });
        modelBuilder.Entity<Counterparty>(entity => { entity.ToTable("counterparties"); });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.Property(e => e.TransactionType)
                .HasConversion<string>()
                .HasMaxLength(32);
            entity.Property(e => e.TransactionStatus)
                .HasConversion<string>()
                .HasMaxLength(32);
            entity.HasOne(e => e.Card)
                .WithMany(c => c.Transactions)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Counterparty)
                .WithMany()
                .HasForeignKey(e => e.CounterpartyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CardUserProfile>(cup =>
        {
            cup.ToTable("card_user_profiles");
            cup.HasKey(e => e.Id);
            cup.HasIndex(e => new { e.CardId, e.UserId }).IsUnique();
            cup.HasOne(e => e.Card)
                .WithMany(c => c.CardUserProfiles)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);
            cup.HasOne(e => e.User)
                .WithMany(u => u.CardUserProfiles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            cup.HasOne(e => e.CardLimit)
                .WithOne(l => l.CardUserProfile)
                .HasForeignKey<CardUserProfile>(e => e.CardLimitId)
                .OnDelete(DeleteBehavior.SetNull);
            cup.HasIndex(e => e.CardLimitId)
                .IsUnique()
                .HasFilter("card_limit_id IS NOT NULL");
        });
    }
}
