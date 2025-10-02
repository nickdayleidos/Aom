using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AgentRoutingProfileView
{
    public int Employeeid { get; set; }

    public string? WeekdayProfile { get; set; }

    public string? WeekendProfile { get; set; }
}
