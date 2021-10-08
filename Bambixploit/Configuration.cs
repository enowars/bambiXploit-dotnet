using System;
using System.Net;
using System.Text.RegularExpressions;

namespace bambiXploit_dotnet
{
    public record Configuration(
        Regex FlagRegex,
        string Command,
        string Arguments,
        int Interval,
        string[] Targets,
        IPAddress SubmissionAddress,
        int SubmissionPort,
        Guid RunGuid);
}
