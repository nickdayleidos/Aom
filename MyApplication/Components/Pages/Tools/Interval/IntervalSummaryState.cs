using System;
using System.Collections.Generic;

namespace MyApplication.Components.Pages.Tools.Interval
{
    public class IntervalSummaryState
    {
        public HeaderSection Header { get; set; } = new();
        public NotesSection Notes { get; set; } = new();
        public CurrentDaySection Current { get; set; } = new();
        public MtdSection Mtd { get; set; } = new();
        public BacklogSection Backlog { get; set; } = new();
        public List<IntervalData> Intervals { get; set; } = new();

        public class IntervalData
        {
            public string IntervalLabel { get; set; } = string.Empty;
            public int CallsOffered { get; set; }
            public int Answered { get; set; }
            public double Asa { get; set; }
        }

        // ===== Header / Interval =====
        public class HeaderSection
        {
            public DateTime? IntervalDate { get; set; } = DateTime.Today;
            public string IntervalStart { get; set; } = "";   // HH:mm
            public string IntervalEnd { get; set; } = "";     // HH:mm
            public DateTime? PopulateTime { get; set; }
        }

        // ===== Notes / Comments - UPDATED NAMES =====
        public class NotesSection
        {
            public string NaTodaysFocusArea { get; set; } = "";
            public string NaMajorCirImpact { get; set; } = "";
            public string NaImpactingEvents { get; set; } = "";
            public string NaHpsmStatus { get; set; } = "";
            public string NaManagementNotes { get; set; } = "";
        }

        // ===== Current Day =====
        public class CurrentDaySection
        {
            public string UsnCallsOffered { get; set; } = "";
            public string UsnCallsAnswered { get; set; } = "";
            public string UsnAsa { get; set; } = "";

            public string VipCallsOffered { get; set; } = "";
            public string VipCallsAnswered { get; set; } = "";
            public string VipAsa { get; set; } = "";

            public string SiprCallsOffered { get; set; } = "";
            public string SiprCallsAnswered { get; set; } = "";
            public string SiprAsa { get; set; } = "";

            public string NnpiCallsOffered { get; set; } = "";
            public string NnpiCallsAnswered { get; set; } = "";
            public string NnpiAsa { get; set; } = "";

            // Email
            public string CurrentEmailCount { get; set; } = "";
            public string CurrentEmailAgeHours { get; set; } = "";
            public string CurrentCustomerCareCount { get; set; } = "";
            public string CurrentCustomerCareAgeHours { get; set; } = "";

            // SIPR
            public string CurrentSiprEmailCount { get; set; } = "";
            public string CurrentSiprEmailAgeHours { get; set; } = "";
            public string CurrentSiprGdaSpreadsheets { get; set; } = "";
            public string CurrentSiprGdaAgeHours { get; set; } = "";
            public string CurrentSiprUaifCount { get; set; } = "";
            public string CurrentSiprUaifAgeHours { get; set; } = "";

            // Voicemail
            public string CurrentVmCount { get; set; } = "";
            public string CurrentVmAgeHours { get; set; } = "";

            // ESS
            public string CurrentEssCount { get; set; } = "";
            public string CurrentEssAgeHours { get; set; } = "";
        }

        // ===== MTD =====
        public class MtdSection
        {
            public string UsnCallsOffered { get; set; } = "";
            public string UsnCallsAnswered { get; set; } = "";
            public string UsnAsa { get; set; } = "";

            public string VipCallsOffered { get; set; } = "";
            public string VipCallsAnswered { get; set; } = "";
            public string VipAsa { get; set; } = "";

            public string SiprCallsOffered { get; set; } = "";
            public string SiprCallsAnswered { get; set; } = "";
            public string SiprAsa { get; set; } = "";

            public string NnpiCallsOffered { get; set; } = "";
            public string NnpiCallsAnswered { get; set; } = "";
            public string NnpiAsa { get; set; } = "";

            // SLR 3.3
            public string Slr33EmLos1 { get; set; } = "";
            public string Slr33EmLos2 { get; set; } = "";
            public string Slr33VmLos1 { get; set; } = "";
            public string Slr33VmLos2 { get; set; } = "";
        }

        // ===== Backlog =====
        public class BacklogSection
        {
            

            // SRM
            public string SrmUaAutoCount { get; set; } = "";
            public string SrmUaAutoAgeDays { get; set; } = "";
            public string SrmUaUsnManCount { get; set; } = "";
            public string SrmUaUsnManAgeDays { get; set; } = "";
            public string SrmUaSocManCount { get; set; } = "";
            public string SrmUaSocManAgeDays { get; set; } = "";

            public string SrmValCount { get; set; } = "";
            public string SrmValAgeDays { get; set; } = "";
            public string SrmValLineFailCount { get; set; } = "";
            public string SrmValLineFailAgeDays { get; set; } = "";
            public string SrmValEmailCount { get; set; } = "";
            public string SrmValEmailAgeDays { get; set; } = "";

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

            public string AfuCount { get; set; } = "";
            public string AfuAgeDays { get; set; } = "";
            public string CsCount { get; set; } = "";
            public string CsAgeDays { get; set; } = "";

            public string RdmUsnCount { get; set; } = "";
            public string RdmUsnAgeDays { get; set; } = "";
            public string RdmEsdUsnCount { get; set; } = "";
            public string RdmEsdUsnAgeDays { get; set; } = "";

            // Other Backlog Queues
            public string NnpiQueue { get; set; } = "";
            public string SiprQueue { get; set; } = "";
            public string NcisQueue { get; set; } = "";
            public string VipQueue { get; set; } = "";
            public string RdmNnpiQueue { get; set; } = "";
            public string RdmSiprQueue { get; set; } = "";
        }
    }
}