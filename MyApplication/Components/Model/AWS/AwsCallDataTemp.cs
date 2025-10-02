using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsCallDataTemp
{
    public string? Contactid { get; set; }

    public string? Initiationmethod { get; set; }

    public string? QueueName { get; set; }

    public string? AgentUsername { get; set; }

    public int? QueueDuration { get; set; }

    public int? AgentAgentinteractionduration { get; set; }

    public int? AgentAftercontactworkduration { get; set; }

    public short? AgentNumberofholds { get; set; }

    public int? AgentCustomerholdduration { get; set; }

    public int? AgentLongestholdduration { get; set; }

    public DateTime? AgentAftercontactworkendtimestamp { get; set; }

    public DateTime? AgentAftercontactworkstarttimestamp { get; set; }

    public DateTime? AgentConnectedtoagenttimestamp { get; set; }

    public string? AgentRoutingprofileName { get; set; }

    public short? Agentconnectionattempts { get; set; }

    public string? Channel { get; set; }

    public DateTime? Connectedtosystemtimestamp { get; set; }

    public string? CustomerendpointAddress { get; set; }

    public DateTime? Disconnecttimestamp { get; set; }

    public string? Initialcontactid { get; set; }

    public DateTime? Initiationtimestamp { get; set; }

    public DateTime? Lastupdatetimestamp { get; set; }

    public string? Nextcontactid { get; set; }

    public string? Previouscontactid { get; set; }

    public DateTime? QueueDequeuetimestamp { get; set; }

    public DateTime? QueueEnqueuetimestamp { get; set; }

    public string? SystemendpointAddress { get; set; }

    public DateTime? Transfercompletedtimestamp { get; set; }

    public string? Transferredtoendpoint { get; set; }

    public string? TransferredtoendpointAddress { get; set; }

    public string? AttributesCallcategory { get; set; }

    public string? AttributesCalltype { get; set; }

    public DateTime? Recordings0Starttimestamp { get; set; }

    public DateTime? Recordings0Stoptimestamp { get; set; }

    public string? AttributesCallbacknumber { get; set; }

    public short? Year { get; set; }

    public byte? Month { get; set; }

    public byte? Day { get; set; }

    public string? AttributesUsername { get; set; }

    public string? AwsGuid { get; set; }

    public int? QueueId { get; set; }

    public int? InitiationMethodId { get; set; }

    public int? ChannelId { get; set; }

    public int? CallTypeId { get; set; }

    public int? AgentId { get; set; }

    public int? RoutingProfileId { get; set; }
}
