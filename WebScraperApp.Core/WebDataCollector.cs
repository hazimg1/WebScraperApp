using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using WebScraperApp.Common.Events;
using WebScraperApp.Common.Models;
using WebScraperApp.Core.Interfaces;

namespace WebScraperApp.Core;

public class WebDataCollector : IWebDataCollector
{
    public event EventHandler<ItemTraversedArgs>? ItemTraversed;
    public event EventHandler<WebPageLoadedArgs>? WebPageLoaded;
    public event EventHandler<ProgressValueChangedArgs>? ProgressValueChanged;

    private readonly Uri baseUri;
    private readonly IHttpClientFactory clientFactory;
    private readonly ILogger<WebDataCollector> logger;
    private readonly IOptions<SiteSettings> settings;

    public WebDataCollector(IHttpClientFactory clientFactory, ILogger<WebDataCollector> logger, IOptions<SiteSettings> settings)
    {
        this.clientFactory = clientFactory;
        this.logger = logger;
        this.settings = settings;
        baseUri = new Uri(settings.Value.Url);
    }

    /// <summary>
    /// Retrieves content for a collection of URLs and returns them as a sequence of URL-content pairs.
    /// </summary>
    /// <param name="urls">A collection of URIs representing the URLs to fetch content from.</param>
    /// <returns>A sequence of URL-content pairs, where each pair contains the URL and its corresponding byte content.</returns>
    public async Task<IEnumerable<(Uri url, byte[] contents)>> GetUrlContent(IEnumerable<Uri> urls)
    {
        try
        {
            using HttpClient httpClient = clientFactory.CreateClient();
            int count = urls.Count();
            int index = 0;
            var items = new ConcurrentBag<(Uri, byte[])>();
            var options = new ParallelOptions { MaxDegreeOfParallelism = 2 };
            await Task.Run(() =>
            {
                Parallel.ForEach(urls, options, async url =>
                {
                    try
                    {
                        var content = await httpClient.GetByteArrayAsync(url);
                        items.Add((url, content));
                        index++;
                        double percent = ((double)index / count) * 100.0;
                        ProgressValueChanged?.Invoke(this, new ProgressValueChangedArgs(index, percent));
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Content not found for URL: {url}", url);
                    }
                });
            });
            return items;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching URL content.");
            return Enumerable.Empty<(Uri, byte[])>();
        }
    }

    /// <summary>
    /// Retrieves a collection of unique URLs by traversing web pages starting from the base URI.
    /// </summary>
    /// <returns>A sequence of unique URIs representing the URLs found during traversal.</returns>
    public async Task<IEnumerable<Uri>> GetURLsAsync()
    {
        using HttpClient httpClient = clientFactory.CreateClient();
        List<Uri> otherResources = [];
        var visited = new HashSet<Uri>();
        await TraversePagesAsync(baseUri, httpClient, visited, otherResources);
        otherResources.Distinct().ToList();

        return otherResources;
    }

    /// <summary>
    /// Recursively traverses web pages, extracting resources and adding them to the list of other resources.
    /// </summary>
    /// <param name="url">The current URL to traverse.</param>
    /// <param name="httpClient">The HTTP client for fetching page content.</param>
    /// <param name="visited">A set of visited URLs to avoid duplicates.</param>
    /// <param name="otherResources">The list of other resources (non-HTML files).</param>
    private async Task TraversePagesAsync(Uri url, HttpClient httpClient, HashSet<Uri> visited, List<Uri> otherResources)
    {
        if (!visited.Add(url)) return;

        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        WebPageLoaded?.Invoke(this, new WebPageLoadedArgs(url, html));
        var resources = GetResources(html, url);
        var links = resources.Where(resource => resource.AbsolutePath.Contains(".html")).ToList();
        var files = resources.Where(resource => !resource.AbsolutePath.Contains(".html")).ToList();
        otherResources.AddRange(files);
        ItemTraversed?.Invoke(this, new ItemTraversedArgs(visited.Count, otherResources.Count));

        foreach (var link in links)
        {
            await TraversePagesAsync(link, httpClient, visited, otherResources);
        }
    }

    /// <summary>
    /// Extracts resources (URIs) from the HTML content.
    /// </summary>
    /// <param name="html">The HTML content of the page.</param>
    /// <param name="url">The base URL of the page.</param>
    /// <returns>A list of unique URIs representing the resources found in the HTML.</returns>
    private static List<Uri> GetResources(string html, Uri url)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        return htmlDocument.DocumentNode.DescendantsAndSelf()
            .Where(node => node.HasAttributes && !node.OuterHtml.Contains("http:"))
            .SelectMany(node => node.Attributes.Where(attribute => attribute.Name == "src" || attribute.Name == "href"))
            .Select(attribute => new Uri(url, attribute.Value))
            .Distinct()
            .ToList();
    }
}
