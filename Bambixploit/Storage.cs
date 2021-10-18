using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bambixploit
{
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
            Directory.CreateDirectory($"{PathPrefix}/{round}");
            var outTask = File.WriteAllLinesAsync($"{PathPrefix}/{round}/{target}.stdout", stdout);
            var errTask = File.WriteAllLinesAsync($"{PathPrefix}/{this.configuration.RunGuid}/{round}/{target}.stderr", stderr);
            await outTask;
            await errTask;
        }
    }
}
