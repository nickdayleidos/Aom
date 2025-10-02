using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsCallQueue
{
    public int QueueId { get; set; }

    public string Queue { get; set; } = null!;

    public string? Description { get; set; }

    public string? Coi { get; set; }

    public bool? Enabled { get; set; }

    public string? Bucket { get; set; }

    public string? CallGroupSpecial { get; set; }

    public bool? IsAsaimpacting { get; set; }

    public bool? IsReportingWeb { get; set; }
}
