using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Security;

namespace MyApplication.Components.Data;

public partial class AomDbContext
{
    partial void ConfigureSecuritySchema(ModelBuilder b)
    {
        b.Entity<AppRole>().ToTable("AppRole", "Security").HasData(
            new AppRole { Id = 1, Name = "Admin" },
            new AppRole { Id = 2, Name = "Manager" },
            new AppRole { Id = 3, Name = "Supervisor" },
            new AppRole { Id = 4, Name = "WFM" },
            new AppRole { Id = 5, Name = "OST" },
            new AppRole { Id = 6, Name = "ViewOnly" }
        );

        b.Entity<AppRoleAssignment>().ToTable("AppRoleAssignment", "Security");
    }
}
