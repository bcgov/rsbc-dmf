namespace MigrationMetrics.Models.MonthlyCountStat
{
    public class UpdateRequest
{

    public DateTimeOffset RecordedTime { get; set; }

    public DateTimeOffset Start { get; set; }

    public DateTimeOffset End { get; set; }

    public string Category { get; set; }

    public int SourceCount { get; set; }
    public int DestinationCount { get; set; }
}
}
