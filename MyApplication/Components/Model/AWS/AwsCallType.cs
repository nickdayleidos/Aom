using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsCallType
{
    public byte Id { get; set; }

    public string CallType { get; set; } = null!;
}
