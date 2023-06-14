namespace MigrationMetrics.Models
{
    public class ChartOptions {
        public double AspectRatio { get; set; }
        }

    public class ChartWrapper {

        public string Type { get; set; }

        public ChartData Data { get; set; }
        public ChartOptions Options { get; set; }

    }

    public class ChartDataSet
    {
        public string Label { get; set; }

        public int[] Data { get; set; }

        public int PointHoverRadius { get; set; }

        public string BorderColor { get; set; }

        public int BorderWidth { get; set; }

        public bool Fill { get; set; }

        public bool DrawActiveElementsOnTop { get; set; }

    }

    public class ChartData
    {
        
      public List<string> Labels { get; set; }

      public List<ChartDataSet> Datasets { get; set; }

      

    }
}
