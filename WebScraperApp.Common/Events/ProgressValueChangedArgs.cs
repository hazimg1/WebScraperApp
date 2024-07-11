namespace WebScraperApp.Common.Events;

public class ProgressValueChangedArgs(int total, double percentage) : EventArgs
{
    public int Total { get; set; } = total;
    public double Percentage { get; set; } = percentage;
}
