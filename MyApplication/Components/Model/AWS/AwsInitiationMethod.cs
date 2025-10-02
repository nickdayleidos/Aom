using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsInitiationMethod
{
    public int Id { get; set; }

    public string InitiationMethod { get; set; } = null!;
}
