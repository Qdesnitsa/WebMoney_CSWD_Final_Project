using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using WebMoney.Data;
using WebMoney.Data.Repositories;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Persistence.Entities;
using WebMoney.Services;

namespace WebMoney
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            const long maxLogFileSizeBytes = 5 * 1024 * 1024;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File(
                    path: "logs/webmoney-.log",
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    fileSizeLimitBytes: maxLogFileSizeBytes,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 10,
                    shared: true)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddDbContext<WebContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
                builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
                builder.Services.AddSingleton<IPasswordHasher<Card>, PasswordHasher<Card>>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<ITransactionService, TransactionService>();
                builder.Services.AddScoped<ICardService, CardService>();
                builder.Services.AddScoped<IUserProfileRepository, UserProfileProfileRepository>();
                builder.Services.AddScoped<ICardRepository, CardRepository>();
                builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
                builder.Services.AddScoped<IDepositTransactionService, DepositTransactionService>();
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });
                builder.Services.AddHttpContextAccessor();
                builder.Logging.AddSerilog(Log.Logger);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseRouting();

                app.UseSession();

                app.UseAuthorization();

                app.MapStaticAssets();
                app.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}")
                    .WithStaticAssets();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Приложение остановлено из-за необработанного исключения при запуске.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}