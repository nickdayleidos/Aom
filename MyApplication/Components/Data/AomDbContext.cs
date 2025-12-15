using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Aws;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Model.AOM.Security;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Service.Employee.Dtos;
using System.Reflection.Emit;

namespace MyApplication.Components.Data
{
    public class AomDbContext : DbContext
    {
        public AomDbContext(DbContextOptions<AomDbContext> options) : base(options) { }

        // =========================
        // Employee schema
        // =========================
        public DbSet<Employees> Employees { get; set; } = default!;
        public DbSet<Employer> Employers { get; set; } = default!;
        public DbSet<Manager> Managers { get; set; } = default!;
        public DbSet<Supervisor> Supervisors { get; set; } = default!;
        public DbSet<Site> Sites { get; set; } = default!;
        public DbSet<Organization> Organizations { get; set; } = default!;
        public DbSet<SubOrganization> SubOrganizations { get; set; } = default!;
        public DbSet<EmployeeHistory> EmployeeHistory { get; set; } = default!;

        // NEW: View
        public DbSet<EmployeeCurrentDetails> EmployeeCurrentDetails { get; set; } = default!;

        // WFM (Employee schema)
        public DbSet<BreakTemplates> BreakTemplates { get; set; } = default!;
        public DbSet<AcrOvertimeSchedules> AcrOvertimeSchedules { get; set; } = default!;
        public DbSet<AcrOvertimeTypes> AcrOvertimeTypes { get; set; } = default!;
        public DbSet<BreakSchedules> BreakSchedules { get; set; } = default!;
        public DbSet<DetailedSchedule> DetailedSchedule { get; set; } = default!;

        // Opera (Employee schema)
        public DbSet<ActivityType> ActivityTypes { get; set; } = default!;
        public DbSet<ActivitySubType> ActivitySubTypes { get; set; } = default!;
        public DbSet<OperaStatus> OperaStatuses { get; set; } = default!;
        public DbSet<OperaRequest> OperaRequests { get; set; } = default!;

        // ACR (Employee schema)
        public DbSet<AcrType> AcrTypes { get; set; } = default!;
        public DbSet<AcrStatus> AcrStatuses { get; set; } = default!;
        public DbSet<AcrRequest> AcrRequests { get; set; } = default!;
        public DbSet<AcrSchedule> AcrSchedules { get; set; } = default!;
        public DbSet<AcrOrganization> AcrOrganizations { get; set; } = default!;

        // Skills (Employee schema)
        public DbSet<Skills> Skills { get; set; } = default!;
        public DbSet<SkillType> SkillType { get; set; } = default!;

        // =========================
        // Tools schema
        // =========================
        public DbSet<EmailTemplates> EmailTemplates { get; set; } = default!;
        public DbSet<IntervalSummary> IntervalSummaries { get; set; } = default!;
        public DbSet<OiCategory> OiCategories { get; set; } = default!;
        public DbSet<OiSeverity> OiSeverities { get; set; } = default!;
        public DbSet<OiEvent> OiEvents { get; set; } = default!;
        public DbSet<OiEventUpdate> OiEventUpdates { get; set; } = default!;
        public DbSet<OiStatus> OiStatuses { get; set; } = default!;
        public DbSet<ProactiveAnnouncement> ProactiveAnnouncements { get; set; } = default!;
        public DbSet<DailyScheduleRow> DailyScheduleRows { get; set; }

        // =========================
        // AWS schema
        // =========================

        public DbSet<Status> Statuses { get; set; } = default!;

        // New AWS Tables
        public DbSet<RoutingProfile> RoutingProfiles { get; set; } = default!;
        public DbSet<CallQueue> CallQueues { get; set; } = default!;
        public DbSet<RoutingProfileQueue> RoutingProfileQueues { get; set; } = default!;
        public DbSet<EmployeeRoutingProfile> EmployeeRoutingProfiles { get; set; } = default!;
        public DbSet<Identifiers> Identifiers { get; set; } = default!;

        // NEW: Security Tables
        public DbSet<AppRole> AppRoles { get; set; } = default!;
        public DbSet<AppRoleAssignment> AppRoleAssignments { get; set; } = default!;
        public DbSet<AwsAgentActivityDto> AwsAgentActivities { get; set; }


        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // AUTO-LOAD CONFIGURATIONS (Loads Employee, EmployeeCurrentDetails, AcrOvertimeSchedules)
            b.ApplyConfigurationsFromAssembly(typeof(AomDbContext).Assembly);

            // =========================================================================================
            // TODO: Move the rest of these configurations to their own files like EmployeeConfiguration
            // =========================================================================================

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

            b.Entity<ActivityType>().ToTable(nameof(ActivityType), "Employee").HasKey(x => x.Id);
            b.Entity<ActivitySubType>().ToTable(nameof(ActivitySubType), "Employee").HasKey(x => x.Id);
            b.Entity<OperaStatus>().ToTable(nameof(OperaStatus), "Employee").HasKey(x => x.Id);
            b.Entity<OperaRequest>()
                 .ToTable(nameof(OperaRequest), "Employee", tb =>
                 {
                     tb.HasTrigger("tr_OperaRequest_ToDetailedSchedule");
                 })
                 .HasKey(x => x.RequestId);

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

            b.Entity<EmailTemplates>().ToTable(nameof(EmailTemplates), "Tools").HasKey(x => x.Id);
            b.Entity<IntervalSummary>().ToTable(nameof(IntervalSummary), "Tools").HasKey(x => x.Id);
            b.Entity<OiCategory>().ToTable(nameof(OiCategory), "Tools").HasKey(x => x.Id);
            b.Entity<OiSeverity>().ToTable(nameof(OiSeverity), "Tools").HasKey(x => x.Id);
            b.Entity<OiEvent>().ToTable(nameof(OiEvent), "Tools").HasKey(x => x.Id);
            b.Entity<OiEventUpdate>().ToTable(nameof(OiEventUpdate), "Tools").HasKey(x => x.Id);
            b.Entity<OiStatus>().ToTable(nameof(OiStatus), "Tools").HasKey(x => x.Id);
            b.Entity<ProactiveAnnouncement>().ToTable(nameof(ProactiveAnnouncement), "Tools").HasKey(x => x.Id);

            b.Entity<Status>().ToTable(nameof(Statuses), "Aws").HasKey(x => x.Id);

            // New AWS Configurations
            b.Entity<RoutingProfile>().ToTable(nameof(RoutingProfile), "Aws").HasKey(x => x.Id);
            b.Entity<CallQueue>().ToTable(nameof(CallQueue), "Aws").HasKey(x => x.Id);
            b.Entity<RoutingProfileQueue>().ToTable(nameof(RoutingProfileQueue), "Aws").HasKey(x => x.Id);
            b.Entity<EmployeeRoutingProfile>().ToTable(nameof(EmployeeRoutingProfile), "Aws").HasKey(x => x.Id);
            b.Entity<Identifiers>().ToTable(nameof(Identifiers), "Aws").HasKey(x => x.Id);

            // Security
            b.Entity<AppRole>().ToTable("AppRole", "Security").HasData(
            new AppRole { Id = 1, Name = "Admin" },
            new AppRole { Id = 2, Name = "Manager" },
            new AppRole { Id = 3, Name = "Supervisor" },
            new AppRole { Id = 4, Name = "WFM" },
            new AppRole { Id = 5, Name = "OST" },
            new AppRole { Id = 6, Name = "ViewOnly" }
        );

            b.Entity<AppRoleAssignment>().ToTable("AppRoleAssignment", "Security");


            b.Entity<DailyScheduleRow>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_DailyScheduleDetails", "Employee");
            });


            // ============================================================
            // RELATIONSHIPS
            // ============================================================
            b.Entity<Manager>(e =>
            {
                
                e.HasOne(m => m.Employee)
                 .WithMany() // Or .WithOne() if a Manager has only 1 Employee record ever
                 .HasForeignKey(m => m.EmployeeId)
                 .OnDelete(DeleteBehavior.NoAction); // Optional: prevents cascade cycles
            });

            // FIX: Changed .HasOne<Employees>() to .HasOne(x => x.Employee) to use the navigation property
            b.Entity<Supervisor>()
             .HasOne(x => x.Employee)
             .WithMany()
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<DetailedSchedule>(e =>
            {
                e.HasOne(x => x.Employees)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
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

            // New AWS Relationships
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
                 .WithMany(p => p.RoutingProfileQueues) // <--- CHANGED: Connect to the new collection
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

            b.Entity<Identifiers>(e =>
            {
                e.HasOne(i => i.Employee)
                 .WithMany()
                 .HasForeignKey(i => i.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            b.Entity<AwsAgentActivityDto>()
                .HasNoKey()
                .ToView(null);
        }
    }
}