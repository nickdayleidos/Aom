using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsagentStatusDaily
{
    public string? Agent { get; set; }

    public string? RoutingProfile { get; set; }

    public string? StartInterval { get; set; }

    public string? EndInterval { get; set; }

    public double? AgentAnswerRate { get; set; }

    public double? AgentIdleTime { get; set; }

    public string? ContactsMissed { get; set; }

    public double? AgentOnContactTime { get; set; }

    public string? ErrorStatusTime { get; set; }

    public double? NonproductiveTime { get; set; }

    public double? Occupancy { get; set; }

    public double? OnlineTime { get; set; }

    public double? BreakTime { get; set; }

    public double? LunchTime { get; set; }

    public double? OlCustomerWorkTime { get; set; }

    public string? MeetingTime { get; set; }

    public double? TrainingTime { get; set; }

    public double? CoachingTime { get; set; }

    public string? PeerMentoringTime { get; set; }

    public double? SystemTime { get; set; }

    public string? AdminTasksTime { get; set; }

    public double? AverageAfterContactWorkTime { get; set; }

    public double? AverageAgentInteractionTime { get; set; }

    public double? AverageCustomerHoldTime { get; set; }

    public double? AverageHandleTime { get; set; }

    public double? ContactsHandled { get; set; }

    public double? ContactsTransferredOutByAgent { get; set; }
}
