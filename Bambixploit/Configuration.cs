﻿namespace Bambixploit
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    public record Configuration(
        Regex FlagRegex,
        string Command,
        string Arguments,
        int Interval,
        string TargetsUrl,
        IPAddress SubmissionAddress,
        int SubmissionPort,
        bool DebugSubmission,
        Guid RunGuid);
}
