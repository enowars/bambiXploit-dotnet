using System;
using System.Net;
using System.Text.RegularExpressions;

namespace bambixploit
{
    public record Configuration(
        Regex FlagRegex,
        string Command,
        string Arguments,
        int Interval,
        string[] Targets,
        IPAddress SubmissionAddress,
        int SubmissionPort,
        bool DebugSubmission,
        Guid RunGuid);
}