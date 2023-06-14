namespace MigrationMetrics.Models
{
    public class VelocityData
    {
        public string Label { get; set; }
        public long Total { get; set; }

        public long Improvement { get; set; }
        public double Time { get; set; }

        public double Velocity { get; set; }

        public string ProjectedEnd { get; set; }
    }
}
