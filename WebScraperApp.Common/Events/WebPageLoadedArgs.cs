namespace WebScraperApp.Common.Events;

public class WebPageLoadedArgs(Uri uri, string content) : EventArgs
{
    public Uri Uri { get; set; } = uri;
    public string Content { get; set; } = content;
}
