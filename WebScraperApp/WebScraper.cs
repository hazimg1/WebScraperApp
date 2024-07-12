
using Microsoft.Extensions.Logging;
using WebScraperApp.Common.Events;
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
        webDataCollector.WebPageLoaded += WebDataCollector_WebPageLoaded;
        webDataCollector.ItemTraversed += WebDataCollector_ItemTraversed;
        webDataCollector.ProgressValueChanged += WebDataCollector_ProgressValueChanged;
        storageManager.ProgressValueChanged += StorageManager_ProgressValueChanged;
    }

    /// <summary>
    /// Scrapes web content, retrieves URLs, fetches their content, and saves it to disk.
    /// </summary>
    public async Task ScrapeWebAsync()
    {
        try
        {
            var urls = await webDataCollector.GetURLsAsync();
            var items = await webDataCollector.GetUrlContent(urls);
            if (items != null)
            {
                await storageManager.SaveOnDiskAsync(items);
            }
            else
            {
                logger.LogWarning("No data can be saved. Check if there are valid items.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while scraping web content.");
        }
    }


    private void WebDataCollector_ItemTraversed(object? sender, ItemTraversedArgs e)
    {
        Console.SetCursorPosition(1, 2);
        Console.WriteLine("Collecting resources: ");
        Console.WriteLine($"Html Traversed and Saved: {e.Html} Other resources Traversed: {e.OtherFiles} Total: {e.Html + e.OtherFiles}");
    }

    private void WebDataCollector_ProgressValueChanged(object? sender, ProgressValueChangedArgs e)
    {
        int loc = (int)e.Percentage;
        Console.SetCursorPosition(100, 6);
        Console.Write("╣");
        Console.SetCursorPosition(loc, 6);
        Console.Write("█");
    }

    private async void WebDataCollector_WebPageLoaded(object? sender, WebPageLoadedArgs e)
    {
        await storageManager.SaveHtmlOnDiskAsync(e.Uri, e.Content);
    }

    private void StorageManager_ProgressValueChanged(object? sender, ProgressValueChangedArgs e)
    {
        int loc = (int)e.Percentage;
        Console.SetCursorPosition(100, 10);
        Console.Write("╣");
        Console.SetCursorPosition(loc, 10);
        Console.Write("█");
    }
}