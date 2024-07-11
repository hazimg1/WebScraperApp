using Microsoft.Extensions.Logging;
using WebScraperApp.Core.Interfaces;
using WebScraperApp.Infrastructure.Interfaces;
using WebScraperApp.Interfaces;

namespace WebScraperApp;

public class WebScraper : IWebScraper
{
    private readonly ILogger<WebScraper> logger;
    private readonly IWebDataCollector webDataCollector;
    private readonly IStorageManager storageManager;


    public WebScraper(ILogger<WebScraper> logger, IWebDataCollector webDataCollector, IStorageManager storageManager)
    {
        this.logger = logger;
        this.webDataCollector = webDataCollector;
        this.storageManager = storageManager;
    }

    public Task ScrapeWebAsync()
    {
        throw new NotImplementedException();
    }
}
