using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AgentProfile
{
    public int Employeeid { get; set; }

    public int? Weekdayprofileid { get; set; }

    public int? Weekendprofileid { get; set; }

    public int? Previousprofileid { get; set; }

    public string? AdditionalSkill { get; set; }
}
