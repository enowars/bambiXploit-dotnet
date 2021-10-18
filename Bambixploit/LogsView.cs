namespace Bambixploit
{
    using System.Collections.Generic;
    using Terminal.Gui;

    public class LogsView : View
    {
        private readonly Storage storage;
        private readonly List<string> roundsList = new();

        public LogsView(Storage storage)
            : base()
        {
            this.storage = storage;
            this.Height = Dim.Fill();
            this.Width = Dim.Fill();
            this.X = 0;
            this.Y = 0;
            this.LayoutStyle = LayoutStyle.Computed;

            /*
            this.rounds = new ListView(roundsList);
            rounds.X = 0;
            rounds.Y = 0;
            rounds.Width = Dim.Fill();
            rounds.Height = Dim.Fill();
            this.Add(rounds);

            this.Update();
            */

            var testlabel = new Label($"The stdout and stderr for each exploit are saved to {this.storage.PathPrefix}.");
            testlabel.X = 0;
            testlabel.Y = 0;
            testlabel.Width = Dim.Fill();
            testlabel.Height = Dim.Fill();
            this.Add(testlabel);
            this.LayoutSubviews();
            this.SetNeedsDisplay();
        }

        private void Update()
        {
            this.roundsList.Clear();
            for (long i = this.storage.LatestRound; i > this.storage.LatestRound - 20; i--)
            {
                if (i < 1)
                {
                    break;
                }

                this.roundsList.Add($"{i}");
            }
        }
    }
}
