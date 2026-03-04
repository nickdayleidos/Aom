using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Aws;
using MyApplication.Components.Service.Employee;

namespace MyApplication.Components.Data;

public partial class AomDbContext
{
    partial void ConfigureAwsSchema(ModelBuilder b)
    {
        // ---- Table mappings ----
        b.Entity<Status>().ToTable(nameof(Statuses), "Aws").HasKey(x => x.Id);
        b.Entity<RoutingProfile>().ToTable(nameof(RoutingProfile), "Aws").HasKey(x => x.Id);
        b.Entity<CallQueue>().ToTable(nameof(CallQueue), "Aws").HasKey(x => x.Id);
        b.Entity<RoutingProfileQueue>().ToTable(nameof(RoutingProfileQueue), "Aws").HasKey(x => x.Id);
        b.Entity<EmployeeRoutingProfile>().ToTable(nameof(EmployeeRoutingProfile), "Aws").HasKey(x => x.Id);
        b.Entity<Identifiers>().ToTable(nameof(Identifiers), "Aws").HasKey(x => x.Id);

        b.Entity<AwsAgentActivityDto>()
            .HasNoKey()
            .ToView(null);

        // ---- Relationships ----
        b.Entity<CallQueue>(e =>
        {
            e.HasOne(q => q.SkillType)
             .WithMany()
             .HasForeignKey(q => q.SkillTypeId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<RoutingProfileQueue>(e =>
        {
            e.HasOne(rpq => rpq.RoutingProfile)
             .WithMany(p => p.RoutingProfileQueues)
             .HasForeignKey(rpq => rpq.RoutingProfileId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(rpq => rpq.CallQueue)
             .WithMany()
             .HasForeignKey(rpq => rpq.CallQueueId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<EmployeeRoutingProfile>(e =>
        {
            e.HasOne(erp => erp.Employee)
             .WithMany()
             .HasForeignKey(erp => erp.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(erp => erp.WeekdayProfile)
             .WithMany()
             .HasForeignKey(erp => erp.WeekdayProfileId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(erp => erp.WeekendProfile)
             .WithMany()
             .HasForeignKey(erp => erp.WeekendProfileId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
