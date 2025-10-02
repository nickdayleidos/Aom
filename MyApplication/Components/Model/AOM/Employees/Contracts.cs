using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyApplication.Components.Model.AOM.Employee
{
    public class CreateEmployeeDto
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    public string? MiddleInitial { get; set; }

        public int? SiteId { get; set; } = null;
        public int? SupervisorId { get; set; } = null;
        public int? ManagerId { get; set; } = null;
    public int? OrganizationId { get; set; } = null; 
    public int? SubOrganizationId { get; set; } = null;
    public int? EmployerId { get; set; } = null;

    public string? UsnOperatorId { get; set; }
    public string? UsnAdminId { get; set; }
    public string? UsnEmail { get; set; }
    public string? CorporateEmail { get; set; }
    public int? CorporateId { get; set; } = null;

    public string? LeidosUserName { get; set; }
    public bool IsRemote { get; set; }
    public string? AwsId { get; set; }
    public int? EDIPI { get; set; }
    public string? FlankSpeedUserName { get; set; }
}


    public class UpdateEmployeeDto
{
    public int Id { get; set; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    public string? MiddleInitial { get; set; }
        public int? SiteId { get; set; } = null;
        public int? SupervisorId { get; set; } = null;
    public int? ManagerId { get; set; } = null;
    public int? OrganizationId { get; set; } = null;
    public int? SubOrganizationId { get; set; } = null;
    public int? EmployerId { get; set; } = null;

    public string? UsnOperatorId { get; set; }
    public string? UsnAdminId { get; set; }
    public string? UsnEmail { get; set; }
    public string? CorporateEmail { get; set; }
    public int? CorporateId { get; set; } = null;

    public string? LeidosUserName { get; set; }
    public bool IsRemote { get; set; }
    public string? AwsId { get; set; }
    public int? EDIPI { get; set; } = null;
    public string? FlankSpeedUserName { get; set; }
}



    public class EmployeeDetailsDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? MiddleInitial { get; set; }

        public int? SiteId { get; set; } = null;
    public int? SupervisorId { get; set; }= null;
    public int? ManagerId { get; set; }= null;
    public int? OrganizationId { get; set; } = null;
    public int? SubOrganizationId { get; set; } = null;
    public int? EmployerId { get; set; } = null;

    public string? UsnOperatorId { get; set; }
    public string? UsnAdminId { get; set; }
    public string? UsnEmail { get; set; }
    public string? CorporateEmail { get; set; }
    public int? CorporateId { get; set; } = null;

    public string? LeidosUserName { get; set; }
    public bool IsRemote { get; set; }
    public string? AwsId { get; set; }
    public int? EDIPI { get; set; } = null;
    public string? FlankSpeedUserName { get; set; }
}

}