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

    // SLR 3.3
    public double Slr33EmMtdLos1 { get; init; }
    public double Slr33EmMtdLos2 { get; init; }
    public double Slr33VmMtdLos1 { get; init; }
    public double Slr33VmMtdLos2 { get; init; }

    // Backlog – Wireless
    public int CurrentEmailCount { get; init; }
    public double CurrentEmailOldest { get; init; }
    public int CurrentCustomerCareCount { get; init; }
    public double CurrentCustomerCareOldest { get; init; }
    public int CurrentSiprEmailCount { get; init; }
    public double CurrentSiprEmailOldest { get; init; }
    public int CurrentSiprGdaSpreadsheets { get; init; }
    public double CurrentSiprGdaOldest { get; init; }
    public int CurrentSiprUaifCount { get; init; }
    public double CurrentSiprUaifOldest { get; init; }

    // Backlog – Wireline
    public int CurrentVmCount { get; init; }
    public double CurrentVmOldest { get; init; }
    public int CurrentEssCount { get; init; }
    public double CurrentEssOldest { get; init; }

    // Backlog – SRM
    public int SrmUaAutoCount { get; init; }
    public double SrmUaAutoAgeHours { get; init; }
    public int SrmUaUsnManCount { get; init; }
    public double SrmUaUsnManAgeHours { get; init; }
    public int SrmUaSocManCount { get; init; }
    public double SrmUaSocManAgeHours { get; init; }

    public int SrmValCount { get; init; }
    public double SrmValAgeDays { get; init; }
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

    // Other Backlog Queues
    public int NnpiQueue { get; init; }
    public int SiprQueue { get; init; }
    public int NcisQueue { get; init; }
    public int VipQueue { get; init; }
    public int RdmNnpiQueue { get; init; }
    public int RdmSiprQueue { get; init; }

    // Notes - UPDATED NAMES
    public string? NaTodaysFocusArea { get; init; }
    public string? NaMajorCirImpact { get; init; }
    public string? NaImpactingEvents { get; init; }
    public string? NaHpsmStatus { get; init; }
    public string? NaManagementNotes { get; init; }

    // Computed property for formatting interval
    public string IntervalLabel =>
        $"{(IntervalStart.Hours):00}:{(IntervalStart.Minutes):00} - {(IntervalEnd.Hours):00}:{(IntervalEnd.Minutes):00} ET";
}