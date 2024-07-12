using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebScraperApp.Common.Events;
using WebScraperApp.Common.Models;
using WebScraperApp.Infrastructure.Interfaces;

namespace WebScraperApp.Infrastructure;

public class StorageManager : IStorageManager
{
    private readonly ILogger<StorageManager> logger;
    private readonly IOptions<SiteSettings> settings;

    public event EventHandler<ProgressValueChangedArgs>? ProgressValueChanged;

    public StorageManager(ILogger<StorageManager> logger, IOptions<SiteSettings> settings)
    {
        this.logger = logger;
        this.settings = settings;
    }

    /// <summary>
    /// Saves the provided content items to disk.
    /// </summary>
    /// <param name="items">A collection of content items, where each item consists of a URL and its corresponding byte content.</param>
    public async Task SaveOnDiskAsync(IEnumerable<(Uri url, byte[] contents)> items)
    {
        try
        {
            foreach (var (url, contents) in items)
            {
                string fileName = Path.GetFullPath(Path.Join(settings.Value.SiteLocationOnDisk, url.AbsolutePath));
                string? dir = Path.GetDirectoryName(fileName);

                if (!string.IsNullOrEmpty(dir))
                {
                    CreateDirectoryIfNotExist(dir);

                    // Use FileStream to write contents to the file asynchronously
                    using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                    {
                        await fileStream.WriteAsync(contents, 0, contents.Length);
                    }

                    double percent = (items.ToList().IndexOf((url, contents)) + 1) / (double)items.Count() * 100;
                    ProgressValueChanged?.Invoke(this, new ProgressValueChangedArgs(items.ToList().IndexOf((url, contents)) + 1, percent));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error saving content to disk.");
        }
    }

    /// <summary>
    /// Saves the provided HTML content to disk for the specified URL.
    /// </summary>
    /// <param name="url">The URL associated with the HTML content.</param>
    /// <param name="contents">The HTML content to be saved.</param>
    public async Task SaveHtmlOnDiskAsync(Uri url, string contents)
    {
        try
        {
            string fileName = Path.GetFullPath(Path.Join(settings.Value.SiteLocationOnDisk, url.AbsolutePath));
            if (!string.IsNullOrWhiteSpace(Path.GetFileName(fileName)))
            {
                string? dir = Path.GetDirectoryName(fileName);
                if (!string.IsNullOrEmpty(dir))
                {
                    CreateDirectoryIfNotExist(dir);
                    await File.WriteAllTextAsync(fileName, contents);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Can not find the content of {fileName}", url);
        }
    }

    private static void CreateDirectoryIfNotExist(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}