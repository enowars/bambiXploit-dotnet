namespace bambiXploit_dotnet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Terminal.Gui;

    public static class Statistics
    {
        private static long okFlags;

        public static long OkFlags => Interlocked.Read(ref okFlags);

        public static List<FlagsStatistic> FlagStatistics { get; set; } = new();

        static Statistics()
        {
            Task.Run(Aggregate);
        }

        public static void AddOkFlags(long newOkFlags)
        {
            Interlocked.Add(ref okFlags, newOkFlags);
        }

        private static async void Aggregate()
        {
            while (true)
            {
                AddOkFlags(1);
                var newFlagStatistics = FlagStatistics.AsEnumerable().ToList();
                newFlagStatistics.Add(new FlagsStatistic(
                    DateTimeOffset.Now,
                    Interlocked.Read(ref okFlags)));
                if (newFlagStatistics.Count > 20)
                {
                    newFlagStatistics.RemoveAt(0);
                }

                FlagStatistics = newFlagStatistics;
                await Task.Delay(1000);
            }
        }
    }
}
