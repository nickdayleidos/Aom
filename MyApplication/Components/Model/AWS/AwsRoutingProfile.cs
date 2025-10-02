using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsRoutingProfile
{
    public int RoutingProfileId { get; set; }

    public string? RoutingProfile { get; set; }

    public string? Description { get; set; }

    public bool? ReqNnpi { get; set; }

    public bool? ReqNcis { get; set; }

    public bool? ReqVip { get; set; }

    public DateTime? LastUpdated { get; set; }

    public string? Coi { get; set; }

    public bool? Enabled { get; set; }
}
