namespace Bambixploit
{
    public record JsonConfiguration(
        string FlagRegex,
        string[] Addresses,
        string SubmissionAddress,
        int SubmissionPort,
        bool DebugSubmission,
        int Interval);
}
