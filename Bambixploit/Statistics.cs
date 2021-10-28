namespace Bambixploit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Statistics
    {
        private static long okFlags;

        static Statistics()
        {
            Task.Run(Aggregate);
        }

        public static long OkFlags => Interlocked.Read(ref okFlags);

        public static List<FlagsStatistic> FlagStatistics { get; set; } = new();

        public static void AddOkFlags(long newOkFlags)
        {
            Interlocked.Add(ref okFlags, newOkFlags);
        }

        private static async void Aggregate()
        {
            while (true)
            {
                var newFlagStatistics = FlagStatistics.AsEnumerable().ToList();
                newFlagStatistics.Add(new FlagsStatistic(
                    DateTimeOffset.Now,
                    Interlocked.Read(ref okFlags)));
                if (newFlagStatistics.Count > 8)
                {
                    newFlagStatistics.RemoveAt(0);
                }

                FlagStatistics = newFlagStatistics;
                await Task.Delay(10000);
            }
        }
    }
}
