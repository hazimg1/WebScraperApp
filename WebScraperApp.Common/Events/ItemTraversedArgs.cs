namespace WebScraperApp.Common.Events;

public class ItemTraversedArgs(int html, int otherFiles) : EventArgs
{
    public int Html { get; set; } = html;
    public int OtherFiles { get; set; } = otherFiles;
}
