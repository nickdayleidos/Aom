using MyApplication.Components.Model.AOM.Employee;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Tools
{
    public class OstPassdown
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int NewEdlId { get; set; }
        public Employees NewEdl { get; set; }

        public int PrevEdlId { get; set; }
        public Employees PrevEdl { get; set; }

        public DateTime PostedTime { get; set; }
        public string PostedBy { get; set; } = string.Empty;

        public DateTime? UpdateTime { get; set; }
        public string? UpdatedBy { get; set; }

        public string? AsaGoal { get; set; }
        public int? CurrentAsa { get; set; }
        public int? CurrentAwsQueue { get; set; }

        public int? CallbackProjectCount { get; set; }
        public string? CallbackProjectStatus { get; set; }

        public DateTime? ReskillTime { get; set; }
        public int? ReskillById { get; set; }
        public Employees? ReskillBy { get; set; }

        public DateTime? ProactiveTime { get; set; }
        public int? ProactiveById { get; set; }
        public Employees? ProactiveBy { get; set; }

        public DateTime? HomeportTime { get; set; }
        public int? HomeportById { get; set; }
        public Employees? HomeportBy { get; set; }

        public DateTime? SharepointTime { get; set; }
        public int? SharepointById { get; set; }
        public Employees? SharepointBy { get; set; }

        public string? OiStatus { get; set; }
        public string? UiStatus { get; set; }
        public string? AioLongTagStatusatus { get; set; } 
        public string? OtmStatus { get; set; }
        public string? NavyInboxJunkStatus { get; set; }

        public int? Los3Count { get; set; }
        public string? Los3Status { get; set; }

        public int? OpenIdleCount { get; set; }
        public string? OpenIdleStatus { get; set; }

        public int? SvdRocCount { get; set; }
        public string? SvdRocStatus { get; set; }

        public int? SvdEtmCount { get; set; }
        public string? SvdEtmStatus { get; set; }

        public string? ManagerComments { get; set; }
        public string? EdlComments { get; set; }
    }
}