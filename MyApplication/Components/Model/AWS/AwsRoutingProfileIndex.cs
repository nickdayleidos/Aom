using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsRoutingProfileIndex
{
    public short Id { get; set; }

    public string RoutingProfileName { get; set; } = null!;
}
