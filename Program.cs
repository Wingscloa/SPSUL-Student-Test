using SPSUL.Models;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using System.IO.Compression;

/// <summary>
/// Vstupní bod aplikace – konfiguruje všechny služby (DI kontejner) a HTTP pipeline.
///
/// Základní pojmy:
///   builder.Services.AddXxx() – registrace služeb do DI kontejneru (pøed spuštìním aplikace)
///   app.UseXxx()              – pøidání middleware do HTTP pipeline (poøadí záleží!)
///
/// Životnosti DI služeb:
///   Singleton  – jedna instance pro celou dobu bìhu aplikace (sdílená mezi všemi uživateli)
///   Scoped     – jedna instance per HTTP request
///   Transient  – nová instance pøi každém injectování
/// </summary>

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

            // Distribuovaná session ukládaná do SQL Server tabulky "Sessions" (funguje ve více instancích)
            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("Default");
                options.SchemaName = "dbo";
                options.TableName = "Sessions";
            });

            // Session konfigurace – cookie pøežije 7 dní a je HttpOnly (JavaScript k ní nemá pøístup)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // DataProtection – explicitní cesta zaruèuje, že klíèe pøežijí restart kontejneru.
            // SetApplicationName zajistí kompatibilitu klíèù pøi redeploymentu nebo škálování.
            // V Dockeru (Production) se klíèe ukládají do volume, lokálnì se použije výchozí cesta.
            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
                    .SetApplicationName("SPSUL");
            }

            // Scoped služby – nová instance per request
            builder.Services.AddScoped<CacheService>();        // per-teacher IMemoryCache wrapper
            builder.Services.AddScoped<SharedService>();       // jméno/ID pøihlášeného uèitele

            // In-memory cache pro èíselníková data (tøídy, pøedmìty, typy) – sdílená singleton
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<LookupCacheService>(); // singleton, cachuje lookup data 5 min
            builder.Services.AddScoped<AuthorizationService>();   // role ? oprávnìní mapping
            builder.Services.AddScoped<AuditService>();           // záznam CUD operací do AuditLogs

            // Response compression (Brotli > GZip) – komprimuje odpovìdi a šetøí bandwidth
            // Brotli dosahuje lepší komprese než GZip, prohlížeèe ho podporují
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    ["application/javascript", "text/css", "application/json", "image/svg+xml"]);
            });

            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
                options.Level = CompressionLevel.Fastest);
            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
                options.Level = CompressionLevel.Fastest);

            // Azurite (lokální emulátor Azure Blob Storage v Dockeru)
            // V produkci: nahradit connection stringem skuteèného Azure Storage úètu
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

            // Anti-forgery ochrana – generuje XSRF token, který musí být pøiložen ke každému POST/PUT/DELETE
            // Header name "X-XSRF-TOKEN" je èten JavaScriptem z cookie a pøikládán k AJAX requestùm
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
            });

            // Add services to the container.
            builder.Services.AddDbContext<SpsulContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

#if DEBUG
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
#else
            builder.Services.AddControllersWithViews();
#endif

            var app = builder.Build();

            // Auto-apply pending EF Core migrations or EnsureCreated (for Docker)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SpsulContext>();
                var useEnsureCreated = builder.Configuration.GetValue<bool>("Database:EnsureCreated");
                if (useEnsureCreated)
                {
                    db.Database.EnsureCreated();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");

                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseHsts();

                // Pøeète X-Forwarded-For / X-Forwarded-Proto posílané reverse proxy (nginx/traefik)
                // Nutné pro správné pøesmìrování a logování IP adres za proxy
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            // Response compression — only in production; in development it causes
            // ERR_CONTENT_DECODING_FAILED when a view-rendering exception corrupts the Brotli stream
            if (!app.Environment.IsDevelopment())
            {
                app.UseResponseCompression();
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

            // Set XSRF token cookie for AJAX requests
            app.Use(async (context, next) =>
            {
                var antiforgery = context.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken ?? "",
                    new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Strict });
                await next();
            });

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}");

            app.Run();
        }
    }
}
