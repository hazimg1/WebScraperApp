
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebScraperApp;
using WebScraperApp.Common.Models;
using WebScraperApp.Core;
using WebScraperApp.Core.Interfaces;
using WebScraperApp.Infrastructure;
using WebScraperApp.Infrastructure.Interfaces;
using WebScraperApp.Interfaces;

class Program
{
    static async Task Main(string[] args)
    {

        var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.Configure<SiteSettings>(context.Configuration.GetSection("SiteSettings"));
            services.AddHttpClient();
            services.AddScoped<IWebDataCollector, WebDataCollector>();
            services.AddScoped<IStorageManager, StorageManager>();
            services.AddScoped<IWebScraper, WebScraper>();
        })
        .Build();

        var webScraper = host.Services.GetRequiredService<IWebScraper>();
        await webScraper.ScrapeWebAsync();
        Console.WriteLine("Program terminated");
    }
}