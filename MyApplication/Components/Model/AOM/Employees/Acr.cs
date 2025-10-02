using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyApplication.Components.Model.AOM.Employee
{
    public class AcrType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<AcrType> AcrTypes { get; set; } = new List<AcrType>();
    }

        public class AcrStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<AcrStatus> AcrStatuses { get; set; } = new List<AcrStatus>();
        }

    public class AcrRequest 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int AcrTypeId { get; set; }
        public string? SubmitterCommment { get; set; }
        public string? WfmComment { get; set; }
        public int? AcrStatusId { get; set; }
        public AcrStatus? AcrStatus { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public DateTime? SubmitTime { get; set; }
        public DateTime? LastUpdateTime { get; set; }
        public ICollection<AcrRequest> AcrRequests { get; set; } = new List<AcrRequest>();

    }
    public class AcrOrganization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AcrRequestId { get; set; }
        public AcrRequest? AcrRequest { get; set; }
        public int? ManagerId { get; set; }
        public Manager? Manager { get; set; }
        public int? SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public int? SubOrganizationId { get; set; }
        public SubOrganization? SubOrganization { get; set; }
        public int? EmployerId { get; set; }
        public Employer? Employer { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLoa { get; set; }
        public bool? IsIntLoa { get; set; }
        public bool? IsRemote { get; set; }
        
    }

    public class AcrSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AcrRequestId { get; set; }
        public AcrRequest? AcrRequest { get; set; }
        public bool? IsSplitSchedule { get; set; }
        public bool? ShiftNumber { get; set; }
        public TimeOnly? MondayStart { get; set; }
        public TimeOnly? MondayEnd { get; set; }
        public TimeOnly? TuesdayStart { get; set; }
        public TimeOnly? TuesdayEnd { get; set; }
        public TimeOnly? WednesdayStart { get; set; }
        public TimeOnly? WednesdayEnd { get; set; }
        public TimeOnly? ThursdayStart { get; set; }
        public TimeOnly? ThursdayEnd { get; set; }
        public TimeOnly? FridayStart { get; set; }
        public TimeOnly? FridayEnd { get; set; }
        public TimeOnly? SaturdayStart { get; set; }
        public TimeOnly? SaturdayEnd { get; set; }
        public TimeOnly? SundayStart { get; set; }
        public TimeOnly? SundayEnd { get; set; }
        public int? BreakTime { get; set; }
        public int? Breaks { get; set; }
        public int? LunchTime { get; set; }
      
    }
}
