using Terminal.Gui;

namespace bambixploit
{
    public class DebugView : View
    {
        public DebugView(Configuration configuration)
        {
            Height = Dim.Fill();
            Width = Dim.Fill();
            X = 0;
            Y = 0;
            LayoutStyle = LayoutStyle.Computed;
            Add(CreateLabel(0, "Configuration"));
            Add(CreateLabel(1, $"Regex: {configuration.FlagRegex}"));
            Add(CreateLabel(2, $"Command: {configuration.Command} {string.Join(' ', configuration.Arguments)}"));
            Add(CreateLabel(3, $"Interval: {configuration.Interval} Seconds"));
            Add(CreateLabel(4, $"Submission: {configuration.SubmissionAddress}:{configuration.SubmissionPort}"));
            Add(CreateLabel(5, $"Run GUID: {configuration.RunGuid}"));
            LayoutSubviews();
            SetNeedsDisplay();
        }

        private static Label CreateLabel(int y, string content)
        {
            var label = new Label(content);
            label.Height = Dim.Fill();
            label.Width = Dim.Fill();
            label.X = 0;
            label.Y = y;
            label.CanFocus = false;

            return label;
        }
    }
}