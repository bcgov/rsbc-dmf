namespace MigrationMetrics.Models
{
    public class ProgressData
    {
        public string Label { get; set; }
        public long OracleCount { get; set; }
        public long DynamicsCount { get; set; }
        public long Difference { get; set; }
        public double Percentage { get; set; }
    }
}
