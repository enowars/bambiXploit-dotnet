using System.Net;
using System.Text.RegularExpressions;

namespace bambiXploit_dotnet
{
    public record JsonConfiguration(
        string FlagRegex,
        string[] Addresses,
        string SubmissionAddress,
        int SubmissionPort,
        int Interval);
}
