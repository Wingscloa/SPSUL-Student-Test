using SPSUL.Models;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
namespace SPSUL
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

            // Azurit
            builder.Services.AddSingleton(x =>
            {
                return new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")!);
            });

            builder.Services.AddScoped<AzureBlobService>();

            // Add services to the container.
            builder.Services.AddDbContext<SpsulContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");

                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 30;
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={durationInSeconds}");
                }
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}");

            app.Run();
        }
    }
}
