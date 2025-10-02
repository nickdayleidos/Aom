using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class Asaquick
{
    public long? AsaTableId { get; set; }

    public int? Year { get; set; }

    public string? Month { get; set; }

    public DateOnly? Date { get; set; }

    public string Coi { get; set; } = null!;

    public int? AnsweredCallsDaily { get; set; }

    public int? Anstime { get; set; }

    public decimal? Asadaily { get; set; }

    public int? CallsOfferedDaily { get; set; }

    public int? CallsAbandonedDaily { get; set; }

    public int? CallsOfferedMtd { get; set; }

    public int? AnsweredCallsMtd { get; set; }

    public decimal? Mtdasa { get; set; }

    public int? CallsAbandonedMtd { get; set; }
}
