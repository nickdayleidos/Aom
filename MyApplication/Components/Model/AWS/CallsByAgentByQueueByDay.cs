using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class CallsByAgentByQueueByDay
{
    public DateOnly? EDate { get; set; }

    public string? QueueName { get; set; }

    public string? AgentUsername { get; set; }

    public int? TotalContacts { get; set; }

    public int? AcdCalls { get; set; }

    public int? OutboundCalls { get; set; }

    public int? Callbacks { get; set; }

    public int? AcdTime { get; set; }

    public int? AcwTime { get; set; }

    public int? HoldTime { get; set; }
}
