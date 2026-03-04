using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Aws;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Service.Employee;

namespace MyApplication.Components.Data;

public partial class AomDbContext
{
    partial void ConfigureEmployeeSchema(ModelBuilder b)
    {
        // ---- Table mappings ----
        b.Entity<Employer>().ToTable(nameof(Employer), "Employee").HasKey(x => x.Id);
        b.Entity<Manager>().ToTable(nameof(Manager), "Employee").HasKey(x => x.Id);
        b.Entity<Supervisor>().ToTable(nameof(Supervisor), "Employee").HasKey(x => x.Id);
        b.Entity<Site>().ToTable(nameof(Site), "Employee").HasKey(x => x.Id);
        b.Entity<Organization>().ToTable(nameof(Organization), "Employee").HasKey(x => x.Id);
        b.Entity<SubOrganization>().ToTable(nameof(SubOrganization), "Employee").HasKey(x => x.Id);
        b.Entity<EmployeeHistory>().ToTable(nameof(EmployeeHistory), "Employee").HasKey(x => x.Id);

        b.Entity<BreakTemplates>().ToTable(nameof(BreakTemplates), "Employee").HasKey(x => x.Id);
        b.Entity<AcrOvertimeTypes>().ToTable("OvertimeTypes", "Employee").HasKey(x => x.Id);
        b.Entity<BreakSchedules>().ToTable(nameof(BreakSchedules), "Employee").HasKey(x => x.Id);
        b.Entity<DetailedSchedule>().ToTable(nameof(DetailedSchedule), "Employee").HasKey(x => x.Id);
        b.Entity<ScheduleType>().ToTable(nameof(ScheduleType), "Employee").HasKey(x => x.Id);

        b.Entity<ActivityType>().ToTable(nameof(ActivityType), "Employee").HasKey(x => x.Id);
        b.Entity<ActivitySubType>().ToTable(nameof(ActivitySubType), "Employee").HasKey(x => x.Id);
        b.Entity<OperaStatus>().ToTable(nameof(OperaStatus), "Employee").HasKey(x => x.Id);
        b.Entity<OperaRequest>()
             .ToTable(nameof(OperaRequest), "Employee", tb =>
             {
                 tb.HasTrigger("tr_OperaRequest_ToDetailedSchedule");
             })
             .HasKey(x => x.RequestId);
        b.Entity<OperaTimeframe>().ToTable(nameof(OperaTimeframe), "Employee").HasKey(x => x.Id);

        b.Entity<AcrType>().ToTable(nameof(AcrType), "Employee").HasKey(x => x.Id);
        b.Entity<AcrStatus>().ToTable(nameof(AcrStatus), "Employee").HasKey(x => x.Id);
        b.Entity<AcrRequest>().ToTable(nameof(AcrRequest), "Employee", tb =>
        {
            tb.HasTrigger("tr_AcrRequest_AutoProcess");
        }).HasKey(x => x.Id);
        b.Entity<AcrSchedule>().ToTable(nameof(AcrSchedule), "Employee").HasKey(x => x.Id);
        b.Entity<AcrOrganization>().ToTable(nameof(AcrOrganization), "Employee").HasKey(x => x.Id);

        b.Entity<Skills>().ToTable(nameof(Skills), "Employee").HasKey(x => x.Id);
        b.Entity<SkillType>().ToTable(nameof(SkillType), "Employee").HasKey(x => x.Id);

        b.Entity<CertificationType>().ToTable("CertificationTypes", "Employee").HasKey(x => x.Id);
        b.Entity<CertificationVendor>().ToTable("CertificationVendors", "Employee").HasKey(x => x.Id);
        b.Entity<Certification>().ToTable("Certifications", "Employee").HasKey(x => x.Id);

        b.Entity<DailyScheduleRow>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_DailyScheduleDetails", "Employee");
        });

        // ---- Relationships ----
        b.Entity<Manager>(e =>
        {
            e.HasOne(m => m.Employee)
             .WithMany()
             .HasForeignKey(m => m.EmployeeId)
             .OnDelete(DeleteBehavior.NoAction);
        });

        b.Entity<Supervisor>()
         .HasOne(x => x.Employee)
         .WithMany()
         .HasForeignKey(x => x.EmployeeId)
         .OnDelete(DeleteBehavior.Restrict);

        b.Entity<CertificationType>(e =>
        {
            e.HasOne(x => x.Vendor)
             .WithMany()
             .HasForeignKey(x => x.VendorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Certification>(e =>
        {
            e.HasOne(x => x.Employee)
             .WithMany()
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.CertificationType)
             .WithMany()
             .HasForeignKey(x => x.CertificationTypeId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Employees>(e =>
        {
            e.HasOne(x => x.Aws)
             .WithOne(i => i.Employee)
             .HasForeignKey<Identifiers>(i => i.EmployeeId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<DetailedSchedule>(e =>
        {
            e.HasOne(x => x.Employees)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ScheduleType)
           .WithMany()
           .HasForeignKey(x => x.ScheduleTypeId)
           .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.BreakTemplate)
           .WithMany()
           .HasForeignKey(x => x.BreakTemplateId)
           .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ActivityType)
            .WithMany()
            .HasForeignKey(x => x.ActivityTypeId)
            .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ActivitySubType)
            .WithMany()
            .HasForeignKey(x => x.ActivitySubTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<BreakSchedules>()
         .HasOne<Employees>()
         .WithMany()
         .HasForeignKey(x => x.EmployeeId)
         .OnDelete(DeleteBehavior.Restrict);

        b.Entity<EmployeeHistory>(e =>
        {
            e.HasOne(x => x.Employee)
             .WithMany()
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.NoAction);

            e.HasOne(x => x.Supervisor)
             .WithMany()
             .HasForeignKey(x => x.SupervisorId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Manager)
             .WithMany()
             .HasForeignKey(x => x.ManagerId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Site)
             .WithMany()
             .HasForeignKey(x => x.SiteId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Employer)
             .WithMany()
             .HasForeignKey(x => x.EmployerId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Organization)
             .WithMany()
             .HasForeignKey(x => x.OrganizationId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.SubOrganization)
             .WithMany()
             .HasForeignKey(x => x.SubOrganizationId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ScheduleRequest)
             .WithMany()
             .HasForeignKey(x => x.ScheduleRequestId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.OvertimeRequest)
               .WithMany()
               .HasForeignKey(x => x.OvertimeRequestId)
               .OnDelete(DeleteBehavior.NoAction);

            e.HasIndex(x => x.EmployeeId);
            e.HasIndex(x => x.OvertimeRequestId);
            e.HasIndex(x => new { x.EmployeeId, x.EffectiveDate });
        });

        b.Entity<ActivitySubType>()
         .HasOne(st => st.ActivityType)
         .WithMany(t => t.SubTypes)
         .HasForeignKey(st => st.ActivityTypeId)
         .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OperaRequest>()
         .HasOne(r => r.ActivityType)
         .WithMany()
         .HasForeignKey(r => r.ActivityTypeId)
         .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OperaRequest>()
         .HasOne(r => r.ActivitySubType)
         .WithMany()
         .HasForeignKey(r => r.ActivitySubTypeId)
         .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OperaRequest>()
        .HasOne(r => r.Timeframe)
        .WithMany()
        .HasForeignKey(r => r.TimeframeId)
        .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OperaRequest>()
         .HasOne(r => r.Employees)
         .WithMany()
         .HasForeignKey(r => r.EmployeeId)
         .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OperaRequest>().HasIndex(r => r.StartTime);
        b.Entity<OperaRequest>().HasIndex(r => new { r.EmployeeId, r.StartTime });

        b.Entity<AcrRequest>(e =>
        {
            e.HasOne(r => r.Employee)
             .WithMany()
             .HasForeignKey(r => r.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(r => r.AcrType)
             .WithMany()
             .HasForeignKey(r => r.AcrTypeId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(r => r.AcrStatus)
             .WithMany()
             .HasForeignKey(r => r.AcrStatusId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(r => r.EffectiveDate);
            e.HasIndex(r => r.AcrTypeId);
            e.HasIndex(r => r.AcrStatusId);
            e.HasIndex(r => new { r.EmployeeId, r.EffectiveDate });
        });

        b.Entity<AcrOrganization>(e =>
        {
            e.HasOne(o => o.AcrRequest)
             .WithOne(r => r.AcrOrganization)
             .HasForeignKey<AcrOrganization>(o => o.AcrRequestId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(o => o.Manager).WithMany().HasForeignKey(o => o.ManagerId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(o => o.Supervisor).WithMany().HasForeignKey(o => o.SupervisorId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(o => o.Organization).WithMany().HasForeignKey(o => o.OrganizationId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(o => o.SubOrganization).WithMany().HasForeignKey(o => o.SubOrganizationId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(o => o.Employer).WithMany().HasForeignKey(o => o.EmployerId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(o => o.Site).WithMany().HasForeignKey(o => o.SiteId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
        });

        b.Entity<AcrSchedule>(e =>
        {
            e.HasOne(s => s.AcrRequest)
              .WithMany(r => r.AcrSchedules)
              .HasForeignKey(s => s.AcrRequestId)
              .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Skills>(e =>
        {
            e.HasOne(s => s.Employee)
             .WithMany()
             .HasForeignKey(s => s.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(s => s.SkillType)
             .WithMany()
             .HasForeignKey(s => s.SkillTypeId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
