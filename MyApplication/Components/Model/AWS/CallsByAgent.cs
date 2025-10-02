using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class CallsByAgent
{
    public DateOnly? EDate { get; set; }

    public TimeOnly? Et { get; set; }

    public string? AgentUsername { get; set; }

    public int? AcdTime { get; set; }

    public int? AcwTime { get; set; }

    public int? HoldTime { get; set; }
}
