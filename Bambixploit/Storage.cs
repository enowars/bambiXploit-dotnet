namespace Bambixploit
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class Storage
    {
        private readonly Configuration configuration;

        public Storage(Configuration configuration)
        {
            this.configuration = configuration;
            Directory.CreateDirectory(this.PathPrefix);
        }

        public string PathPrefix => $"./.bambixploit/{this.configuration.RunGuid}";

        public long LatestRound { get; set; } = 10;

        public async Task SaveLogs(List<string> stdout, List<string> stderr, long round, string target)
        {
            Directory.CreateDirectory($"{this.PathPrefix}/{round}");
            var outTask = File.WriteAllLinesAsync($"{this.PathPrefix}/{round}/{target}.stdout", stdout);
            var errTask = File.WriteAllLinesAsync($"{this.PathPrefix}/{round}/{target}.stderr", stderr);
            await outTask;
            await errTask;
        }
    }
}
