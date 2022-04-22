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
        private long minFlags;

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
            this.AxisY.LabelGetter = (v) => (v.Value + this.minFlags).ToString("N0");

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
                var oldestStatistic = currentStatistics[0];
                var latestStatistic = currentStatistics[^1];
                var flagsDiff = latestStatistic.OkFlags - oldestStatistic.OkFlags;
                var minTimestamp = this.begin = oldestStatistic.Timestamp;
                var maxTimestamp = latestStatistic.Timestamp;
                var timestampDiff = maxTimestamp - minTimestamp;
                var yEnd = latestStatistic.OkFlags - (latestStatistic.OkFlags % 10) + 10;
                var yStart = this.minFlags = oldestStatistic.OkFlags - (oldestStatistic.OkFlags % 10);
                var roundedDiff = yEnd - yStart;

                foreach (var flagStatistic in currentStatistics)
                {
                    float okFlags = (float)flagStatistic.OkFlags;
                    var timestampOffset = flagStatistic.Timestamp - minTimestamp;
                    this.line.Points.Add(
                        new PointF(
                            (float)timestampOffset.TotalSeconds,
                            okFlags - this.minFlags));
                }

                this.AxisY.Increment = (float)roundedDiff / 25;
                this.AxisY.ShowLabelsEvery = 5;

                this.AxisX.Increment = 10;
                this.AxisX.ShowLabelsEvery = 1;

                this.ScrollOffset = new PointF(0, 0);
                float cellWidth = Math.Max(0.00000001f, (float)timestampDiff.TotalSeconds / Math.Max(1, this.Frame.Width));
                float cellHeight = (float)(Math.Max(10.0, roundedDiff) / Math.Max(1, this.Frame.Height));

                if (cellHeight == 0)
                {
                    Console.WriteLine("wtf");
                }

                this.CellSize = new PointF(
                    cellWidth,
                    cellHeight);

                this.SetNeedsDisplay();
                await Task.Delay(1000);
            }
        }

        private static int RoundUp(int input, int modulo)
        {
            if (input % modulo == 0)
            {
                return input;
            }

            return modulo - (input % modulo) + input;
        }
    }
}
