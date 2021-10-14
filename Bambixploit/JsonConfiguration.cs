using System.Net;
using System.Text.RegularExpressions;

namespace bambixploit
{
    public record JsonConfiguration(
        string FlagRegex,
        string[] Addresses,
        string SubmissionAddress,
        int SubmissionPort,
        bool DebugSubmission,
        int Interval);
}
