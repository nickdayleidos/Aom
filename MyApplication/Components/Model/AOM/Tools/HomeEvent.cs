using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApplication.Components.Model.AOM.Tools;

[Table("HomeEvent", Schema = "Tools")]
public class HomeEvent
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column(TypeName = "nvarchar(200)")]
    public string Title { get; set; } = "";

    public DateTime EventDate { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "varchar(100)")]
    public string CreatedBy { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}
