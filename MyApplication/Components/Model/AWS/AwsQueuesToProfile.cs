using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsQueuesToProfile
{
    public short? Priority { get; set; }

    public short? Delay { get; set; }

    public int? RoutingProfileId { get; set; }

    public int? QueueId { get; set; }

    public int QueueProfileId { get; set; }
}
