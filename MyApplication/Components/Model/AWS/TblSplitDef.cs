using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class TblSplitDef
{
    public int TblSplitDefId { get; set; }

    public int? CmsEquivalent { get; set; }

    public string? Coi { get; set; }

    public string? Definition { get; set; }

    public string? Bucket { get; set; }

    public string? CallGroup { get; set; }

    public string? CallGroupSpecial { get; set; }

    public double? IsAsaimpacting { get; set; }

    public double? IsReportingWeb { get; set; }
}
