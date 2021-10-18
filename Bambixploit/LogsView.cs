using System.Collections.Generic;
using Terminal.Gui;

namespace bambixploit
{
    public class LogsView : View
    {
        private readonly List<string> roundsList = new();
        private readonly Storage storage;

        public LogsView(Storage storage)
        {
            this.storage = storage;
            Height = Dim.Fill();
            Width = Dim.Fill();
            X = 0;
            Y = 0;
            LayoutStyle = LayoutStyle.Computed;

            /*
            this.rounds = new ListView(roundsList);
            rounds.X = 0;
            rounds.Y = 0;
            rounds.Width = Dim.Fill();
            rounds.Height = Dim.Fill();
            this.Add(rounds);

            this.Update();
            */

            var testlabel =
                new Label($"The stdout and stderr for each exploit are saved to {this.storage.PathPrefix}.");
            testlabel.X = 0;
            testlabel.Y = 0;
            testlabel.Width = Dim.Fill();
            testlabel.Height = Dim.Fill();
            Add(testlabel);


            LayoutSubviews();
            SetNeedsDisplay();
        }

        private void Update()
        {
            roundsList.Clear();
            for (var i = storage.LatestRound; i > storage.LatestRound - 20; i--)
            {
                if (i < 1) break;

                roundsList.Add($"{i}");
            }
        }
    }
}