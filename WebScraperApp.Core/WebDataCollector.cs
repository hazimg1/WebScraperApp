using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

    public Task<IEnumerable<(Uri url, byte[] contents)>> GetUrlContent(IEnumerable<Uri> urls)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Uri>> GetURLsAsync()
    {
        throw new NotImplementedException();
    }
}