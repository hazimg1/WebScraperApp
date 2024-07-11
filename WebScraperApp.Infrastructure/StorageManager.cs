using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebScraperApp.Common.Events;
using WebScraperApp.Common.Models;
using WebScraperApp.Infrastructure.Interfaces;

namespace WebScraperApp.Infrastructure;

public class StorageManager : IStorageManager
{
    public event EventHandler<ProgressValueChangedArgs>? ProgressValueChanged;

    private readonly ILogger<StorageManager> logger;
    private readonly IOptions<SiteSettings> settings;

    public StorageManager(ILogger<StorageManager> logger, IOptions<SiteSettings> settings)
    {
        this.logger = logger;
        this.settings = settings;
    }
    public Task SaveHtmlOnDiskAsync(Uri url, string contents)
    {
        throw new NotImplementedException();
    }

    public Task SaveOnDiskAsync(IEnumerable<(Uri url, byte[] contents)> items)
    {
        throw new NotImplementedException();
    }
}
