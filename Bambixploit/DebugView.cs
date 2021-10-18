namespace Bambixploit
{
    using Terminal.Gui;

    public class DebugView : View
    {
        public DebugView(Configuration configuration)
            : base()
        {
            this.Height = Dim.Fill();
            this.Width = Dim.Fill();
            this.X = 0;
            this.Y = 0;
            this.LayoutStyle = LayoutStyle.Computed;
            this.Add(CreateLabel(0, "Configuration"));
            this.Add(CreateLabel(1, $"Regex: {configuration.FlagRegex}"));
            this.Add(CreateLabel(2, $"Command: {configuration.Command} {string.Join(' ', configuration.Arguments)}"));
            this.Add(CreateLabel(3, $"Interval: {configuration.Interval} Seconds"));
            this.Add(CreateLabel(4, $"Submission: {configuration.SubmissionAddress}:{configuration.SubmissionPort}"));
            this.Add(CreateLabel(5, $"Run GUID: {configuration.RunGuid}"));
            this.LayoutSubviews();
            this.SetNeedsDisplay();
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
