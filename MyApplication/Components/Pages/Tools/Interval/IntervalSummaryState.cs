namespace MyApplication.Components.Pages.Tools.Interval
{
    /// <summary>
    /// Root view-model for the Interval Summary page.
    /// Grouped into sections so markup can bind like State.Header.IntervalStart, State.Current.UsnAsa, etc.
    /// </summary>
    public class IntervalSummaryState
    {
        public HeaderSection Header { get; set; } = new();
        public NotesSection Notes { get; set; } = new();
        public CurrentDaySection Current { get; set; } = new();
        public MtdSection Mtd { get; set; } = new();
        public BacklogSection Backlog { get; set; } = new();

        // ===== Header / Interval =====
        public class HeaderSection
        {
            // Use DateTime so MudDatePicker binds cleanly
            public DateTime? IntervalDate { get; set; } = DateTime.Today;
            public string IntervalStart { get; set; } = "";   // HH:mm
            public string IntervalEnd { get; set; } = "";   // HH:mm
        }

        // ===== Notes / Comments =====
        public class NotesSection
        {
            public string FocusArea { get; set; } = "Reducing backlogs; meeting daily and monthly ASA goals.";
            public string CirImpactAsa { get; set; } = "None.";
            public string ImpactEvents { get; set; } = "None.";
            public string HpsmStatus { get; set; } = "HPSM has been fully operational throughout the interval.";
            public string ManagementNotes { get; set; } = "None.";
        }

        // ===== Current Day =====
        public class CurrentDaySection
        {
            // SvD Total
            public string UsnAsa { get; set; } = "";
            public string UsnCallsOffered { get; set; } = "";
            public string UsnCallsAnswered { get; set; } = "";

            // VIP (O365)
            public string VipAsa { get; set; } = "";
            public string VipCallsOffered { get; set; } = "";
            public string VipCallsAnswered { get; set; } = "";

            // SIPR
            public string SiprAsa { get; set; } = "";
            public string SiprCallsOffered { get; set; } = "";
            public string SiprCallsAnswered { get; set; } = "";

            // NNPI
            public string NnpiAsa { get; set; } = "";
            public string NnpiCallsOffered { get; set; } = "";
            public string NnpiCallsAnswered { get; set; } = "";

            // Email / Cust Care / SIPR Email
            public string EmailCount { get; set; } = "";
            public string EmailOldestHours { get; set; } = "";
            public string CustCareCount { get; set; } = "";
            public string CustCareOldestHours { get; set; } = "";
            public string SiprEmailCount { get; set; } = "";
            public string SiprEmailOldestHours { get; set; } = "";

            // SIPR GDA / UAIF
            public string SiprGdaCount { get; set; } = "";
            public string SiprGdaOldestHours { get; set; } = "";
            public string SiprUaifCount { get; set; } = "";
            public string SiprUaifOldestDays { get; set; } = "";

            // VM / ESS
            public string VmCount { get; set; } = "";
            public string VmOldestHours { get; set; } = "";
            public string EssCount { get; set; } = "";
            public string EssOldestHours { get; set; } = "";
        }

        // ===== Month To Date =====
        public class MtdSection
        {
            // SvD Total
            public string UsnAsa { get; set; } = "";
            public string UsnCallsOffered { get; set; } = "";
            public string UsnCallsAnswered { get; set; } = "";

            // VIP (O365)
            public string VipAsa { get; set; } = "";
            public string VipCallsOffered { get; set; } = "";
            public string VipCallsAnswered { get; set; } = "";

            // SIPR
            public string SiprAsa { get; set; } = "";
            public string SiprCallsOffered { get; set; } = "";
            public string SiprCallsAnswered { get; set; } = "";

            // NNPI
            public string NnpiAsa { get; set; } = "";
            public string NnpiCallsOffered { get; set; } = "";
            public string NnpiCallsAnswered { get; set; } = "";

            // SLR33
            public string Slr33EmLos1 { get; set; } = "";
            public string Slr33EmLos2 { get; set; } = "";
            public string Slr33VmLos1 { get; set; } = "";
            public string Slr33VmLos2 { get; set; } = "";
        }

        // ===== Backlog =====
        public class BacklogSection
        {
            // SRM – user admin
            public string SrmAutoCount { get; set; } = "";
            public string SrmAutoAgeHours { get; set; } = "";
            public string SrmUsnManCount { get; set; } = "";
            public string SrmUsnManAgeHours { get; set; } = "";
            public string SrmSocManCount { get; set; } = "";
            public string SrmSocManAgeHours { get; set; } = "";

            // SRM – validation
            public string SrmValLineCount { get; set; } = "";
            public string SrmValLineAgeDays { get; set; } = "";
            public string SrmValLineFailCount { get; set; } = "";
            public string SrmValLineFailAgeDays { get; set; } = "";
            public string SrmValEmailCount { get; set; } = "";
            public string SrmValEmailAgeDays { get; set; } = "";

            // SRM – OCM Activations (Ready/Hold/Fatal) NIPR/SIPR
            public string OcmNiprReadyCount { get; set; } = "";
            public string OcmNiprReadyAgeHours { get; set; } = "";
            public string OcmSiprReadyCount { get; set; } = "";
            public string OcmSiprReadyAgeHours { get; set; } = "";
            public string OcmNiprHoldCount { get; set; } = "";
            public string OcmNiprHoldAgeHours { get; set; } = "";
            public string OcmSiprHoldCount { get; set; } = "";
            public string OcmSiprHoldAgeHours { get; set; } = "";
            public string OcmNiprFatalCount { get; set; } = "";
            public string OcmNiprFatalAgeHours { get; set; } = "";
            public string OcmSiprFatalCount { get; set; } = "";
            public string OcmSiprFatalAgeHours { get; set; } = "";

            // SRM – AFU / CS
            public string AfuCount { get; set; } = "";
            public string AfuAgeHours { get; set; } = "";
            public string CsCount { get; set; } = "";
            public string CsAgeHours { get; set; } = "";

            // RDM
            public string RdmUsnCount { get; set; } = "";
            public string RdmUsnAgeDays { get; set; } = "";
            public string RdmEsdUsnCount { get; set; } = "";
            public string RdmEsdUsnAgeDays { get; set; } = "";
        }
    }
}
