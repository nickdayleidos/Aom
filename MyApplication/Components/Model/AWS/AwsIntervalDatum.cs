using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsIntervalDatum
{
    public DateOnly EDate { get; set; }

    public DateTime Et { get; set; }

    public TimeOnly Interval { get; set; }

    public int QueueId { get; set; }

    public string InitiationMethod { get; set; } = null!;

    public string QueueName { get; set; } = null!;

    public int CallsOffered { get; set; }

    public int Acdcalls { get; set; }

    public int Abncalls { get; set; }

    public long Anstime { get; set; }

    public long Acdtime { get; set; }

    public long Acwtime { get; set; }

    public int Holdcalls { get; set; }

    public long Holdtime { get; set; }

    public int Voicemails { get; set; }

    public int Callbacks { get; set; }

    public long Abntime { get; set; }

    public int DistinctAgent { get; set; }

    public bool IsAsaimpacting { get; set; }

    public bool IsReportingWeb { get; set; }

    public long AbnTimeAsa { get; set; }

    public int AbnCallsAsa { get; set; }
}
