using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MigrationMetrics.Entities;

    [Index(nameof(RecordedTime))]
public class MonthlyCountStat
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime RecordedTime { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Category { get; set; }
    
    public int SourceCount { get; set; }
    public int DestinationCount { get; set; }

    }

