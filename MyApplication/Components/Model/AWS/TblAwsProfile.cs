using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class TblAwsProfile
{
    public string? Coi { get; set; }

    public string? RoutingProfile { get; set; }

    public string? TemplateDesc { get; set; }

    public bool? Nnpi { get; set; }

    public bool? Ncis { get; set; }

    public bool? Vip { get; set; }

    public DateTime? QueueUpdate { get; set; }
}
