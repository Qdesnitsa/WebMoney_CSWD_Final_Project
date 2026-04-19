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
    public DbSet<Counterparty> Counterparties { get; set; }

    public DbSet<RoleLookup> RoleLookups { get; set; }
    public DbSet<CardStatusLookup> CardStatusLookups { get; set; }
    public DbSet<CurrencyCodeLookup> CurrencyCodeLookups { get; set; }
    public DbSet<TransactionTypeLookup> TransactionTypeLookups { get; set; }
    public DbSet<TransactionStatusLookup> TransactionStatusLookups { get; set; }

    public WebContext(DbContextOptions<WebContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoleLookup>(e =>
        {
            e.ToTable("roles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Code).HasMaxLength(64).IsRequired();
            e.HasData(
                new RoleLookup { Id = 1, Code = "User" },
                new RoleLookup { Id = 2, Code = "Admin" });
        });

        modelBuilder.Entity<CardStatusLookup>(e =>
        {
            e.ToTable("card_statuses");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Code).HasMaxLength(64).IsRequired();
            e.HasData(
                new CardStatusLookup { Id = 1, Code = "Initialized" },
                new CardStatusLookup { Id = 2, Code = "Active" },
                new CardStatusLookup { Id = 3, Code = "Expired" },
                new CardStatusLookup { Id = 4, Code = "Blocked" });
        });

        modelBuilder.Entity<CurrencyCodeLookup>(e =>
        {
            e.ToTable("currency_codes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Code).HasMaxLength(16).IsRequired();
            e.HasData(
                new CurrencyCodeLookup { Id = 1, Code = "BYN" },
                new CurrencyCodeLookup { Id = 2, Code = "USD" },
                new CurrencyCodeLookup { Id = 3, Code = "EURO" });
        });

        modelBuilder.Entity<TransactionTypeLookup>(e =>
        {
            e.ToTable("transaction_types");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Code).HasMaxLength(64).IsRequired();
            e.HasData(
                new TransactionTypeLookup { Id = 1, Code = "Withdrawal" },
                new TransactionTypeLookup { Id = 2, Code = "Deposit" });
        });

        modelBuilder.Entity<TransactionStatusLookup>(e =>
        {
            e.ToTable("transaction_statuses");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Code).HasMaxLength(64).IsRequired();
            e.HasData(
                new TransactionStatusLookup { Id = 1, Code = "Initialized" },
                new TransactionStatusLookup { Id = 2, Code = "Pending" },
                new TransactionStatusLookup { Id = 3, Code = "Completed" },
                new TransactionStatusLookup { Id = 4, Code = "Canceled" },
                new TransactionStatusLookup { Id = 5, Code = "Failed" });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasConversion<int>();
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
            entity.Property(e => e.CurrencyCode).HasConversion<int>();
            entity.Property(e => e.CardStatus).HasConversion<int>();
        });

        modelBuilder.Entity<CardLimit>(entity => { entity.ToTable("card_limits"); });

        modelBuilder.Entity<Counterparty>(entity => { entity.ToTable("counterparties"); });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.Property(e => e.TransactionType).HasConversion<int>();
            entity.Property(e => e.TransactionStatus).HasConversion<int>();
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
            cup.HasIndex(e => new { e.CardId, e.UserProfileId }).IsUnique();
            cup.HasOne(e => e.Card)
                .WithMany(c => c.CardUserProfiles)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);
            cup.HasOne(e => e.UserProfile)
                .WithMany(up => up.CardUserProfiles)
                .HasForeignKey(e => e.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            cup.HasOne(e => e.CardLimit)
                .WithMany(cl => cl.CardUserProfiles)
                .HasForeignKey(e => e.CardLimitId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}