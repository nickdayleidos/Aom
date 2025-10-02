using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsDetailedAgentDatum
{
    public Guid EventId { get; set; }

    public string AwsId { get; set; } = null!;

    public DateTime EventTimeEt { get; set; }

    public string EventType { get; set; } = null!;

    public string CurrentAgentStatus { get; set; } = null!;

    public string CurrentStatusType { get; set; } = null!;

    public string PrevAgentStatus { get; set; } = null!;

    public string PrevStatusType { get; set; } = null!;

    public string CurrentRoutingProfile { get; set; } = null!;

    public string PrevRoutingProfile { get; set; } = null!;

    public string? ChangeType { get; set; }

    public DateTime? EndTime { get; set; }

    public int? Duration { get; set; }

    public string? AwsGuid { get; set; }
}
