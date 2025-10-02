using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsAhttarget
{
    public int AhtTargetId { get; set; }

    public string QueueName { get; set; } = null!;

    public DateOnly AhtTargetMonth { get; set; }

    public int AcdCalls { get; set; }

    public int Tht { get; set; }

    public decimal EntrAhtTarget { get; set; }
}
