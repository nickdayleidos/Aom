using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsDetailedAgentDataTemp
{
    public Guid? Eventid { get; set; }

    public string? AwsId { get; set; }

    public DateTime? EventTimeEt { get; set; }

    public string? EventType { get; set; }

    public string? CurrentAgentStatus { get; set; }

    public string? CurrentStatusType { get; set; }

    public string? PrevAgentStatus { get; set; }

    public string? PrevStatusType { get; set; }

    public string? CurrentRoutingProfile { get; set; }

    public string? PrevRoutingProfile { get; set; }

    public string? ChangeType { get; set; }

    public string? AwsGuid { get; set; }
}
