using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WebMoney.Data;

public class WebContextFactory : IDesignTimeDbContextFactory<WebContext>
{
    public WebContext CreateDbContext(string[] args)
    {
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        var basePath = Path.GetDirectoryName(typeof(WebContext).Assembly.Location)
                       ?? Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<WebContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        return new WebContext(optionsBuilder.Options);
    }
}

