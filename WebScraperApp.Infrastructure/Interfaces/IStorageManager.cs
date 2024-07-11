using WebScraperApp.Common.Events;

namespace WebScraperApp.Infrastructure.Interfaces;

public interface IStorageManager
{
    event EventHandler<ProgressValueChangedArgs>? ProgressValueChanged;
    Task SaveOnDiskAsync(IEnumerable<(Uri url, byte[] contents)> items);
    Task SaveHtmlOnDiskAsync(Uri url, string contents);
}