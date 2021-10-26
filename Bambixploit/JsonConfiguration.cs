namespace Bambixploit
{
    public record JsonConfiguration(
        string FlagRegex,
        string TargetsUrl,
        string SubmissionAddress,
        int SubmissionPort,
        bool DebugSubmission,
        int Interval);
}
