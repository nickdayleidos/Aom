// Components/Services/Email/IntervalEmailContext.cs
namespace MyApplication.Components.Services.Email;

public sealed class IntervalEmailContext
{
    public DateTime DateLocal { get; init; }
    public TimeSpan IntervalStart { get; init; }
    public TimeSpan IntervalEnd { get; init; }

    // Current day (page: CurrentDay)
    public int UsnCallsOffered { get; init; }
    public int UsnCallsAnswered { get; init; }
    public double UsnAsa { get; init; }

    public int VipCallsOffered { get; init; }
    public int VipCallsAnswered { get; init; }
    public double VipAsa { get; init; }

    public int SiprCallsOffered { get; init; }
    public int SiprCallsAnswered { get; init; }
    public double SiprAsa { get; init; }

    public int NnpiCallsOffered { get; init; }
    public int NnpiCallsAnswered { get; init; }
    public double NnpiAsa { get; init; }

    // MTD (page: MonthToDate)
    public int MtdUsnCallsOffered { get; init; }
    public int MtdUsnCallsAnswered { get; init; }
    public double MtdUsnAsa { get; init; }

    public int MtdVipCallsOffered { get; init; }
    public int MtdVipCallsAnswered { get; init; }
    public double MtdVipAsa { get; init; }

    public int MtdSiprCallsOffered { get; init; }
    public int MtdSiprCallsAnswered { get; init; }
    public double MtdSiprAsa { get; init; }

    public int MtdNnpiCallsOffered { get; init; }
    public int MtdNnpiCallsAnswered { get; init; }
    public double MtdNnpiAsa { get; init; }

    // SLR33 MTD (%)
    public double Slr33EmLos1 { get; init; }
    public double Slr33EmLos2 { get; init; }
    public double Slr33VmLos1 { get; init; }
    public double Slr33VmLos2 { get; init; }

    // Email / Cust Care / SIPR Email / GDA / UAIF
    public int EmailCount { get; init; }
    public double EmailOldestHours { get; init; }
    public int CustCareCount { get; init; }
    public double CustCareOldestHours { get; init; }
    public int SiprEmailCount { get; init; }
    public double SiprEmailOldestHours { get; init; }
    public int SiprGdaCount { get; init; }
    public double SiprGdaOldestHours { get; init; }
    public int SiprUaifCount { get; init; }
    public double SiprUaifOldestDays { get; init; }

    // VM / ESS
    public int VmCount { get; init; }
    public double VmOldestHours { get; init; }
    public int EssCount { get; init; }
    public double EssOldestHours { get; init; }

    // Backlog – SRM User Admin / Validation / AFU / Incidents
    public int SrmAutoCount { get; init; }
    public double SrmAutoAgeHours { get; init; }
    public int SrmUsnManCount { get; init; }
    public double SrmUsnManAgeHours { get; init; }
    public int SrmSocManCount { get; init; }
    public double SrmSocManAgeHours { get; init; }

    public int SrmValLineCount { get; init; }
    public double SrmValLineAgeDays { get; init; }
    public int SrmValLineFailCount { get; init; }
    public double SrmValLineFailAgeDays { get; init; }
    public int SrmValEmailCount { get; init; }
    public double SrmValEmailAgeDays { get; init; }

    public int AfuCount { get; init; }
    public double AfuAgeHours { get; init; }
    public int CsCount { get; init; }
    public double CsAgeHours { get; init; }

    // OCM – NIPR/SIPR Ready/Hold/Fatal
    public int OcmNiprReadyCount { get; init; }
    public double OcmNiprReadyAgeHours { get; init; }
    public int OcmSiprReadyCount { get; init; }
    public double OcmSiprReadyAgeHours { get; init; }

    public int OcmNiprHoldCount { get; init; }
    public double OcmNiprHoldAgeHours { get; init; }
    public int OcmSiprHoldCount { get; init; }
    public double OcmSiprHoldAgeHours { get; init; }

    public int OcmNiprFatalCount { get; init; }
    public double OcmNiprFatalAgeHours { get; init; }
    public int OcmSiprFatalCount { get; init; }
    public double OcmSiprFatalAgeHours { get; init; }

    // RDM
    public int RdmUsnCount { get; init; }
    public double RdmUsnAgeDays { get; init; }
    public int RdmEsdUsnCount { get; init; }
    public double RdmEsdUsnAgeDays { get; init; }

    // Notes
    public string? FocusArea { get; init; }
    public string? CirImpactAsa { get; init; }
    public string? ImpactEvents { get; init; }
    public string? HpsmStatus { get; init; }
    public string? ManagementNotes { get; init; }

    public string IntervalLabel => $"{IntervalStart:hh\\:mm}–{IntervalEnd:hh\\:mm}";
}
public sealed class IntervalRow
{
    public string IntervalLabel { get; init; } = "";
    public int CallsOffered { get; init; }
    public int Answered { get; init; }
    public int ASA { get; init; } // seconds
}