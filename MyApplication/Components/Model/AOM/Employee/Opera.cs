using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace MyApplication.Components.Model.AOM.Employee
{
    public class OperaType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Desc { get; set; }

        public ICollection<OperaSubType> SubTypes { get; set; } = new List<OperaSubType>();
    }

    public class OperaSubType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int OperaTypeId { get; set; }
        public bool IsImpacting { get; set; }
        public string? Desc { get; set; }

        public OperaType OperaType { get; set; } = default!;
        public ICollection<OperaSubClass> SubClasses { get; set; } = new List<OperaSubClass>();
    }

    public class OperaSubClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int OperaSubTypeId { get; set; }

        public OperaSubType OperaSubType { get; set; } = default!;
    }

    public class OperaStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
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
        public int OperaTypeId { get; set; }
        public OperaType OperaType { get; set; } = default!;
        public int OperaSubTypeId { get; set; }
        public int? OperaSubClassId { get; set; }
        public string? SubmitterComments { get; set; }
        public bool Approved { get; set; }
        public string? ApprovedBy { get; set; }
        public bool ReviewedWfm { get; set; }
        public string? ReviewedBy { get; set; }
        public string? WfmComments { get; set; }
        public DateTime SubmitTime { get; set; }

       
      
        public OperaSubType OperaSubType { get; set; } = default!;
        public OperaSubClass? OperaSubClass { get; set; }
    }
}
