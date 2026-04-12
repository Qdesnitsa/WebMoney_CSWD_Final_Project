using Microsoft.EntityFrameworkCore;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data;

public class WebContext : DbContext
{
    public DbSet<UserProfile> Users { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CardLimit> CardLimits { get; set; }
    public DbSet<IdentityDocument> IdentityDocuments { get; set; }

    public WebContext(DbContextOptions<WebContext> options) : base(options)
    {
    }
}