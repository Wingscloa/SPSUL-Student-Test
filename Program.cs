using SPSUL.Models;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace SPSUL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("Default");
                options.SchemaName = "dbo";
                options.TableName = "Sessions";
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped<CacheService>();
            builder.Services.AddScoped<SharedService>();

            // Azurit - s nastavením kompatibilní API verze
            builder.Services.AddSingleton(x =>
            {
                var options = new BlobClientOptions()
                {
                    // Použít starší API verzi kompatibilní s Azurite
                    Retry = 
                    {
                        MaxRetries = 3,
                        Mode = Azure.Core.RetryMode.Exponential
                    }
                };
                
                return new BlobServiceClient(
                    builder.Configuration.GetConnectionString("Azurit")!, 
                    options
                );
            });

            builder.Services.AddScoped<AzureBlobService>();
            builder.Services.AddScoped<PdfService>();

            // Add services to the container.
            builder.Services.AddDbContext<SpsulContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

#if DEBUG
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
#else
            builder.Services.AddControllersWithViews();
#endif

            var app = builder.Build();

            // Auto-apply pending EF Core migrations (for Docker / production)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SpsulContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");

                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 30;
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={durationInSeconds}");
                }
            });

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}");

            app.Run();
        }
    }
}
