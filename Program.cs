using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsExplorerApp.Data;
using NewsExplorerApp.Options;
using NewsExplorerApp.Services;
using NewsExplorerApp.Services.Interfaces;
using System.Net.Http.Headers;

namespace NewsExplorerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
          
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();


            builder.Services.Configure<NewsApiOptions>(
                 builder.Configuration.GetSection(NewsApiOptions.SectionName));

            builder.Services.AddHttpClient<INewsApiClient, NewsApiClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<NewsApiOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseUrl);

                client.DefaultRequestHeaders.UserAgent.ParseAdd("NewsExplorerApp/1.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddScoped<INewsService, NewsService>();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
