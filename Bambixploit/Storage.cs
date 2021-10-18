using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace bambixploit
{
    public class Storage
    {
        private readonly Configuration configuration;

        public Storage(Configuration configuration)
        {
            this.configuration = configuration;
            Directory.CreateDirectory(PathPrefix);
        }

        public string PathPrefix => $"./.bambixploit/{configuration.RunGuid}";

        public long LatestRound { get; set; } = 10;

        public async Task SaveLogs(List<string> stdout, List<string> stderr, long round, string target)
        {
            Directory.CreateDirectory($"{PathPrefix}/{round}");
            var outTask = File.WriteAllLinesAsync($"{PathPrefix}/{round}/{target}.stdout", stdout);
            var errTask =
                File.WriteAllLinesAsync($"{PathPrefix}/{configuration.RunGuid}/{round}/{target}.stderr", stderr);
            await outTask;
            await errTask;
        }
    }
}