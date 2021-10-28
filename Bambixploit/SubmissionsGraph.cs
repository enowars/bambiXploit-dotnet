namespace Bambixploit
{
    using System;
    using System.Threading.Tasks;
    using Terminal.Gui;
    using Terminal.Gui.Graphs;

    public class SubmissionsGraph : GraphView
    {
        private readonly PathAnnotation line = new();
        private DateTimeOffset begin = DateTimeOffset.UtcNow;

        public SubmissionsGraph()
            : base()
        {
            this.CanFocus = false;
            this.X = 0;
            this.Y = 0;
            this.Width = Dim.Fill();
            this.Height = Dim.Fill();

            this.Visible = true;
            this.ColorScheme = Colors.Menu;

            // Draw line first so it does not draw over top of points or axis labels
            this.line.BeforeSeries = true;
            this.Annotations.Add(this.line);

            // leave space for axis labels
            this.MarginBottom = 2;
            this.MarginLeft = 3;

            this.AxisX.Text = "Time (UTC)";
            this.AxisX.LabelGetter = (v) =>
            {
                var end = DateTimeOffset.UtcNow;
                return this.begin.AddSeconds(v.Value).ToString("HH:mm");
            };

            // this.AxisY.Text = "Flags";
            this.AxisY.LabelGetter = (v) => v.Value.ToString("N0");

            this.SetNeedsDisplay();
            this.Update();
        }

        private async void Update()
        {
            while (true)
            {
                var currentStatistics = Statistics.FlagStatistics;
                var count = currentStatistics.Count;
                if (count < 1)
                {
                    continue;
                }

                this.line.Points.Clear();
                var tickHeight = this.Frame.Width / 5;
                var minTimestamp = this.begin = currentStatistics[0].Timestamp;
                var minOkFlags = currentStatistics[0].OkFlags;
                var maxTimestamp = currentStatistics[^1].Timestamp;
                var maxOkFlags = currentStatistics[^1].OkFlags;
                var timestampDiff = maxTimestamp - minTimestamp;
                var flagsDiff = maxOkFlags - minOkFlags;
                var yEnd = Math.Pow(10, Math.Ceiling(Math.Log10(maxOkFlags)));

                foreach (var flagStatistic in currentStatistics)
                {
                    float okFlags = (float)flagStatistic.OkFlags;
                    var timestampOffset = flagStatistic.Timestamp - minTimestamp;
                    this.line.Points.Add(
                        new PointF(
                            (float)timestampOffset.TotalSeconds,
                            okFlags));
                }

                this.AxisY.Increment = (float)yEnd / 5;
                this.AxisY.ShowLabelsEvery = 1;

                this.AxisX.Increment = 10;
                this.AxisX.ShowLabelsEvery = 1;

                this.ScrollOffset = new PointF(0, 0);
                float cellWidth = Math.Max(0.00000001f, (float)timestampDiff.TotalSeconds / Math.Max(1, this.Frame.Width));
                float cellHeight = (float)(1.1f * Math.Max(10.0, yEnd) / Math.Max(1, this.Frame.Height));

                if (cellHeight == 0)
                {
                    Console.WriteLine("wtf");
                }

                this.CellSize = new PointF(
                    cellWidth,
                    cellHeight);

                this.SetNeedsDisplay();
                Console.WriteLine("########");
                await Task.Delay(1000);
            }
        }
    }
}
