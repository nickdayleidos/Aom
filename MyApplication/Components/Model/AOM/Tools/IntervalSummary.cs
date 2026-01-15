using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Tools
{
    public partial class IntervalSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // header
        public DateTime IntervalDate { get; set; }
        public TimeSpan IntervalStart { get; set; }
        public TimeSpan IntervalEnd { get; set; }
        public string? CurrentUser { get; set; }

        // Current – SvD/USN
        public int CurrentUsnASA { get; set; }
        public int CurrentUsnCallsOffered { get; set; }
        public int CurrentUsnCallsAnswered { get; set; }

        // Current – VIP
        public int CurrentVipASA { get; set; }
        public int CurrentVipCallsOffered { get; set; }
        public int CurrentVipCallsAnswered { get; set; }

        // Current – SIPR
        public int CurrentSiprASA { get; set; }
        public int CurrentSiprCallsOffered { get; set; }
        public int CurrentSiprCallsAnswered { get; set; }

        // Current – NNPI
        public int CurrentNnpiASA { get; set; }
        public int CurrentNnpiCallsOffered { get; set; }
        public int CurrentNnpiCallsAnswered { get; set; }

        // MTD – SvD/USN
        public int MtdUsnASA { get; set; }
        public int MtdUsnCallsOffered { get; set; }
        public int MtdUsnCallsAnswered { get; set; }

        // MTD – VIP
        public int MtdVipASA { get; set; }
        public int MtdVipCallsOffered { get; set; }
        public int MtdVipCallsAnswered { get; set; }

        // MTD – SIPR
        public int MtdSiprASA { get; set; }
        public int MtdSiprCallsOffered { get; set; }
        public int MtdSiprCallsAnswered { get; set; }

        // MTD – NNPI
        public int MtdNnpiASA { get; set; }
        public int MtdNnpiCallsOffered { get; set; }
        public int MtdNnpiCallsAnswered { get; set; }

        // SLR 3.3
        [Column(TypeName = "decimal")]
        public decimal? Slr33EmMtdLos1 { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? Slr33EmMtdLos2 { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? Slr33VmMtdLos1 { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? Slr33VmMtdLos2 { get; set; }

        // Backlog – Wireless
        public int CurrentEmailCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? CurrentEmailOldest { get; set; }
        public int CurrentCustomerCareCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? CurrentCustomerCareOldest { get; set; }
        public int CurrentSiprEmailCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? CurrentSiprEmailOldest { get; set; }
        public int CurrentSiprGdaSpreadsheets { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? CurrentSiprGdaOldest { get; set; }
        public int CurrentSiprUaifCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? CurrentSiprUaifOldest { get; set; }

        // Backlog – Wireline
        public int CurrentVmCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? CurrentVmOldest { get; set; }
        public int CurrentEssCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? CurrentEssOldest { get; set; }

        // Backlog – SRM
        public int BlSrmUaAutoCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlSrmUaAutoOldest { get; set; }
        public int BlSrmUaUsnManCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlSrmUaUsnManOldest { get; set; }
        public int BlSrmUaSocManCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlSrmUaSocManOldest { get; set; }

        public int BlSrmValidationCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlSrmValidationOldest { get; set; }
        public int BlSrmValidationFailCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlSrmValidationFailOldest { get; set; }
        public int BlSrmEmailBuildoutsCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlSrmEmailBuildoutsOldest { get; set; }

        public int BlSrmAfuCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlSrmAfuOldest { get; set; }
        public int BlSrmCxSatCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlSrmCxSatOldest { get; set; }

        // OCM
        public int BlOcmNiprReadyCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlOcmNiprReadyOldest { get; set; }
        public int BlOcmSiprReadyCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlOcmSiprReadyOldest { get; set; }

        public int BlOcmNiprHoldCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlOcmNiprHoldOldest { get; set; }
        public int BlOcmSiprHoldCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlOcmSiprHoldOldest { get; set; }

        public int BlOcmNiprFatalCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlOcmNiprFatalOldest { get; set; }
        public int BlOcmSiprFatalCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlOcmSiprFatalOldest { get; set; }

        // RDM
        public int BlRdmUsnCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlRdmUsnOldest { get; set; }
        public int BlRdmUsnEsdCount { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? BlRdmUsnEsdOldest { get; set; }

        // Other Backlog Queues
        public int? NnpiQueue { get; set; }
        public int? SiprQueue { get; set; }
        public int? NcisQueue { get; set; }
        public int? VipQueue { get; set; }
        public int? RdmNnpiQueue { get; set; }
        public int? RdmSiprQueue { get; set; }

        // Notes - UPDATED NAMES
        public string? NaTodaysFocusArea { get; set; }
        public string? NaMajorCirImpact { get; set; }
        public string? NaImpactingEvents { get; set; }
        public string? NaHpsmStatus { get; set; }
        public string? NaManagementNotes { get; set; }
    }
}