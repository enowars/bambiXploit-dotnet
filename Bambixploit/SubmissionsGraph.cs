using System;
using System.Threading.Tasks;
using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace bambixploit
{
    public class SubmissionsGraph : GraphView
    {
        private readonly PathAnnotation line = new();
        private DateTimeOffset begin = DateTimeOffset.UtcNow;

        public SubmissionsGraph()
        {
            CanFocus = false;
            X = 0;
            Y = 0;
            Width = Dim.Fill();
            Height = Dim.Fill();

            Visible = true;
            ColorScheme = Colors.Menu;

            // Draw line first so it does not draw over top of points or axis labels
            line.BeforeSeries = true;
            Annotations.Add(line);

            // leave space for axis labels
            MarginBottom = 2;
            MarginLeft = 3;

            AxisX.Text = "Time (UTC)";
            AxisX.LabelGetter = v =>
            {
                var end = DateTimeOffset.UtcNow;
                return begin.AddSeconds(v.Value).ToString("HH:mm");
            };

            // this.AxisY.Text = "Flags";
            AxisY.LabelGetter = v => v.Value.ToString("N2");

            SetNeedsDisplay();
            Update();
        }

        private async void Update()
        {
            while (true)
            {
                var currentStatistics = Statistics.FlagStatistics;
                var count = currentStatistics.Count;
                if (count < 2) continue;

                line.Points.Clear();
                var tickHeight = Frame.Width / 5;
                var minTimestamp = begin = currentStatistics[0].Timestamp;
                var minOkFlags = currentStatistics[0].OkFlags;
                var maxTimestamp = currentStatistics[^1].Timestamp;
                var maxOkFlags = currentStatistics[^1].OkFlags;
                var timestampDiff = maxTimestamp - minTimestamp;
                var flagsDiff = maxOkFlags - minOkFlags;


                var yEnd = Math.Pow(10, Math.Ceiling(Math.Log10(maxOkFlags)));

                foreach (var flagStatistic in currentStatistics)
                {
                    float okFlags = flagStatistic.OkFlags;
                    var timestampOffset = flagStatistic.Timestamp - minTimestamp;
                    line.Points.Add(
                        new PointF(
                            (float) timestampOffset.TotalSeconds,
                            okFlags));
                }

                AxisY.Increment = (float) yEnd / 5;
                AxisY.ShowLabelsEvery = 1;

                AxisX.Increment = 10;
                AxisX.ShowLabelsEvery = 1;

                ScrollOffset = new PointF(0, 0);
                var cellWidth = Math.Max(0.00000001f, (float) timestampDiff.TotalSeconds / Math.Max(1, Frame.Width));
                var cellHeight = (float) (1.1f * Math.Max(10.0, yEnd) / Math.Max(1, Frame.Height));

                if (cellHeight == 0) Console.WriteLine("wtf");
                CellSize = new PointF(
                    cellWidth,
                    cellHeight);

                SetNeedsDisplay();
                await Task.Delay(1000);
            }
        }
    }
}