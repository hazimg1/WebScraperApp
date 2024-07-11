namespace WebScraperApp.Common.Models;

public record SiteSettings
{
    public required string SiteLocationOnDisk { get; set; }
    public required string Url { get; set; }
}