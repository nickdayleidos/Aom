using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsChannel
{
    public byte Id { get; set; }

    public string Channel { get; set; } = null!;
}
