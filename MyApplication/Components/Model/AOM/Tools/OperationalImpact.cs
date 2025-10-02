using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Tools;

public sealed class OiCategory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Column(TypeName = "varchar(64)")]
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public sealed class OiSeverity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;

}


public sealed class OiEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int SeverityId { get; set; }
    public int SiteId { get; set; }

    public string? TicketNumber { get; set; }
    public string Summary { get; set; } = "";
    public string? ServicesAffected { get; set; }
    public int UsersAffected { get; set; } = 0;
    [Column(TypeName = "varchar(64)")]
    public string? EstimatedTimeToResolve { get; set; }
    public DateTime PostedTime { get; set; }
    public DateTime StartTime { get; set; }
    public string? Description { get; set; }
    public DateTime? ResolutionTime { get; set; }
}

public sealed class OiEventUpdate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int EventId { get; set; }
    public DateTime UpdateTime { get; set; }
    public string Summary { get; set; } = "";
}

public sealed class OiStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int Name { get; set; }
   
}
