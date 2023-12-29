using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SQLite;

namespace MigrationMetrics.Entities;

[Index(nameof(RecordedTime))]
[Index(nameof(Category))]
[Index(nameof(Start))]
[Index(nameof(End))]

public class MonthlyCountStat
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Indexed]
    public DateTime RecordedTime { get; set; }

    [Indexed]
    public DateTime Start { get; set; }

    [Indexed]
    public DateTime End { get; set; }

    [Indexed]
    public string Category { get; set; }
    
    public int SourceCount { get; set; }
    public int DestinationCount { get; set; }

    }

