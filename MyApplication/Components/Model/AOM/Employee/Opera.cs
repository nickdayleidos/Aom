using MyApplication.Components.Model.AOM.Aws;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace MyApplication.Components.Model.AOM.Employee
{
    public class ActivityType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        
        public ICollection<ActivitySubType> SubTypes { get; set; } = new List<ActivitySubType>();
        public bool? IsActive { get; set; } = true;
    }

    public class ActivitySubType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; } = default!;
        public int? AwsStatusId { get; set; }
        public Status? AwsStatus { get; set; }
        public bool? IsActive { get; set; } = true;
    }

    

    public class OperaStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public bool? IsActive { get; set; } = true;
    }

    public class OperaRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        public int EmployeeId { get; set; }
        public Employees? Employees { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SubmittedBy { get; set; } = default!;
        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; } = default!;
        public int ActivitySubTypeId { get; set; }
        public string? SubmitterComments { get; set; }
        public bool Approved { get; set; }
        public string? ApprovedBy { get; set; }
        public bool? ReviewedWfm { get; set; }
        public string? ReviewedBy { get; set; }
        public string? WfmComments { get; set; }
        public DateTime SubmitTime { get; set; }
        public int? OperaStatusId { get; set; }
        public OperaStatus? OperaStatus { get; set; }

        public ActivitySubType? ActivitySubType { get; set; } = default!;
        public DateTime? ApproveTime { get; set; }
        public string? ApproveBy { get; set; }
        public DateTime? CancelledTime { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? RejectedTime { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public string? LastUpdatedBy { get; set; }
        public bool? IsImpacting { get; set; }
        public int? TimeframeId { get; set; }
        public OperaTimeframe? Timeframe { get; set; }

    }

    public class OperaTimeframe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
