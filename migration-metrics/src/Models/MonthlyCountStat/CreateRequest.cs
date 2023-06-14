namespace MigrationMetrics.Models.MonthlyCountStat;


using System.ComponentModel.DataAnnotations;
using MigrationMetrics.Entities;

public class CreateRequest
{
    [Required]
    public DateTime RecordedTime { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    [Required]
    public string Category { get; set; }

    [Required]
    public int SourceCount { get; set; }

    [Required]
    public int DestinationCount { get; set; }

}