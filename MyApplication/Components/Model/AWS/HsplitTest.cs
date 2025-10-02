using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class HsplitTest
{
    public DateOnly? EDate { get; set; }

    public DateTime? Et { get; set; }

    public string? Interval { get; set; }

    public double? CmsEquivalent { get; set; }

    public string? Initiationmethod { get; set; }

    public string? QueueName { get; set; }

    public int? Callsoffered { get; set; }

    public int? Acdcalls { get; set; }

    public int? Abncalls { get; set; }

    public int? Anstime { get; set; }

    public int? Maxanstime { get; set; }

    public int? Acdtime { get; set; }

    public int? Acwtime { get; set; }

    public int? Holdcalls { get; set; }

    public int? Holdtime { get; set; }

    public int? Voicemails { get; set; }

    public int? Callbacks { get; set; }

    public int? Abntime { get; set; }

    public int? DistinctAgent { get; set; }

    public double? IsAsaimpacting { get; set; }

    public double? IsReportingWeb { get; set; }

    public int? AbntimeAsa { get; set; }

    public int? AbncallsAsa { get; set; }

    public int? AcdcallsAsa { get; set; }

    public int? AnstimeAsa { get; set; }
}
