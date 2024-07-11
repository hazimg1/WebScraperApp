
using WebScraperApp.Common.Events;

namespace WebScraperApp.Core.Interfaces;

public interface IWebDataCollector
{
    event EventHandler<ItemTraversedArgs>? ItemTraversed;
    event EventHandler<WebPageLoadedArgs>? WebPageLoaded;
    event EventHandler<ProgressValueChangedArgs>? ProgressValueChanged;
    Task<IEnumerable<(Uri url, byte[] contents)>> GetUrlContent(IEnumerable<Uri> urls);
    Task<IEnumerable<Uri>> GetURLsAsync();
}
