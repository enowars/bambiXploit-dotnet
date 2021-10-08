namespace bambiXploit_dotnet
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Terminal.Gui;

    public record FlagsStatistic(
        DateTimeOffset Timestamp,
        long OkFlags
    );
}
