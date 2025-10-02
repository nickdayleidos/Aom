using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class TblSplitDefOld
{
    public short TblSplitDefId { get; set; }

    public int? CmsEquivalent { get; set; }

    public string Coi { get; set; } = null!;

    public string Definition { get; set; } = null!;

    public string Bucket { get; set; } = null!;

    public string? CallGroup { get; set; }

    public string? CallGroupSpecial { get; set; }

    public bool IsAsaimpacting { get; set; }

    public bool IsReportingWeb { get; set; }
}
