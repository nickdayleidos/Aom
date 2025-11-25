using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Aws;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Model.AOM.Tools;

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

        // WFM (Employee schema)
        public DbSet<BreakTemplates> BreakTemplates { get; set; } = default!;
        public DbSet<AcrOvertimeSchedules> AcrOvertimeSchedules { get; set; } = default!;
        public DbSet<AcrOvertimeTypes> AcrOvertimeTypes { get; set; } = default!; // <- added
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

        // =========================
        // AWS schema
        // =========================

        public DbSet<Status> Statuses { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ------------------------------------------------------------
            // EMPLOYEE core tables
            // ------------------------------------------------------------
            b.Entity<Employees>().ToTable(nameof(Employees), "Employee").HasKey(x => x.Id);
            b.Entity<Employer>().ToTable(nameof(Employer), "Employee").HasKey(x => x.Id);
            b.Entity<Manager>().ToTable(nameof(Manager), "Employee").HasKey(x => x.Id);
            b.Entity<Supervisor>().ToTable(nameof(Supervisor), "Employee").HasKey(x => x.Id);
            b.Entity<Site>().ToTable(nameof(Site), "Employee").HasKey(x => x.Id);
            b.Entity<Organization>().ToTable(nameof(Organization), "Employee").HasKey(x => x.Id);
            b.Entity<SubOrganization>().ToTable(nameof(SubOrganization), "Employee").HasKey(x => x.Id);
            b.Entity<EmployeeHistory>().ToTable(nameof(EmployeeHistory), "Employee").HasKey(x => x.Id);

            // ------------------------------------------------------------
            // WFM
            // ------------------------------------------------------------
            b.Entity<BreakTemplates>().ToTable(nameof(BreakTemplates), "Employee").HasKey(x => x.Id);
            b.Entity<AcrOvertimeTypes>().ToTable("OvertimeTypes", "Employee").HasKey(x => x.Id);
            b.Entity<AcrOvertimeSchedules>().ToTable(nameof(AcrOvertimeSchedules), "Employee").HasKey(x => x.Id);
            b.Entity<BreakSchedules>().ToTable(nameof(BreakSchedules), "Employee").HasKey(x => x.Id);
            b.Entity<DetailedSchedule>().ToTable(nameof(DetailedSchedule), "Employee").HasKey(x => x.Id);

            // ------------------------------------------------------------
            // OPERA
            // ------------------------------------------------------------
            b.Entity<ActivityType>().ToTable(nameof(ActivityType), "Employee").HasKey(x => x.Id);
            b.Entity<ActivitySubType>().ToTable(nameof(ActivitySubType), "Employee").HasKey(x => x.Id);
            b.Entity<OperaStatus>().ToTable(nameof(OperaStatus), "Employee").HasKey(x => x.Id);
            b.Entity<OperaRequest>()
                 .ToTable(nameof(OperaRequest), "Employee", tb =>
                 {
                     tb.HasTrigger("tr_OperaRequest_ToDetailedSchedule");
                 })
                 .HasKey(x => x.RequestId);

            // ------------------------------------------------------------
            // ACR
            // ------------------------------------------------------------
            b.Entity<AcrType>().ToTable(nameof(AcrType), "Employee").HasKey(x => x.Id);
            b.Entity<AcrStatus>().ToTable(nameof(AcrStatus), "Employee").HasKey(x => x.Id);
            b.Entity<AcrRequest>().ToTable(nameof(AcrRequest), "Employee", tb =>
            {
                tb.HasTrigger("tr_AcrRequest_AutoProcess");
            }).HasKey(x => x.Id);

            b.Entity<AcrSchedule>().ToTable(nameof(AcrSchedule), "Employee").HasKey(x => x.Id);
            b.Entity<AcrOrganization>().ToTable(nameof(AcrOrganization), "Employee").HasKey(x => x.Id);

            // ------------------------------------------------------------
            // Skills
            // ------------------------------------------------------------
            b.Entity<Skills>().ToTable(nameof(Skills), "Employee").HasKey(x => x.Id);
            b.Entity<SkillType>().ToTable(nameof(SkillType), "Employee").HasKey(x => x.Id);
            // ------------------------------------------------------------
            // TOOLS
            // ------------------------------------------------------------
            b.Entity<EmailTemplates>().ToTable(nameof(EmailTemplates), "Tools").HasKey(x => x.Id);
            b.Entity<IntervalSummary>().ToTable(nameof(IntervalSummary), "Tools").HasKey(x => x.Id);
            b.Entity<OiCategory>().ToTable(nameof(OiCategory), "Tools").HasKey(x => x.Id);
            b.Entity<OiSeverity>().ToTable(nameof(OiSeverity), "Tools").HasKey(x => x.Id);
            b.Entity<OiEvent>().ToTable(nameof(OiEvent), "Tools").HasKey(x => x.Id);
            b.Entity<OiEventUpdate>().ToTable(nameof(OiEventUpdate), "Tools").HasKey(x => x.Id);
            b.Entity<OiStatus>().ToTable(nameof(OiStatus), "Tools").HasKey(x => x.Id);
            b.Entity<ProactiveAnnouncement>().ToTable(nameof(ProactiveAnnouncement), "Tools").HasKey(x => x.Id);

            // ------------------------------------------------------------
            // AWS
            // ------------------------------------------------------------

            b.Entity<Status>().ToTable(nameof(Statuses), "Aws").HasKey(x => x.Id);


            // ============================================================
            // RELATIONSHIPS: EMPLOYEE / WFM
            // ============================================================
            b.Entity<Manager>()
             .HasOne<Employees>()
             .WithMany()
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Supervisor>()
             .HasOne<Employees>()
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
            }
            ); 

            // Removed: AcrOvertimeSchedules -> Employees (there is no EmployeeId on this table)

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

                // Schedule FK → AcrRequest (ScheduleRequestId holds the ACR id for schedule)
                e.HasOne(x => x.ScheduleRequest)
                 .WithMany()
                 .HasForeignKey(x => x.ScheduleRequestId)
                 .OnDelete(DeleteBehavior.Restrict);
                
                e.HasOne(x => x.OvertimeRequest)                        // NEW
                   .WithMany()
                   .HasForeignKey(x => x.OvertimeRequestId)
                   .OnDelete(DeleteBehavior.NoAction);


                e.HasIndex(x => x.EmployeeId);
                e.HasIndex(x => x.OvertimeRequestId);                  // NEW (lookup speed)
                e.HasIndex(x => new { x.EmployeeId, x.EffectiveDate });
            });

            // ============================================================
            // RELATIONSHIPS: OPERA
            // ============================================================
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

            // ============================================================
            // RELATIONSHIPS: ACR
            // ============================================================
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
                 .WithMany()
                 .HasForeignKey(o => o.AcrRequestId)
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
                // Keep ShiftNumber optional if you allow "null or 1" for first segment; otherwise make required.
                // e.Property(x => x.ShiftNumber).IsRequired();

                e.HasOne(s => s.AcrRequest)
                 .WithMany()
                 .HasForeignKey(s => s.AcrRequestId)
                 .OnDelete(DeleteBehavior.Cascade);

                // e.HasIndex(s => new { s.AcrRequestId, s.ShiftNumber }).IsUnique();
            });

            // ============================================================
            // RELATIONSHIPS: Skills
            // ============================================================

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

            // ============================================================
            // RELATIONSHIPS: OVERTIME SCHEDULES
            // ============================================================
            b.Entity<AcrOvertimeSchedules>(e =>
            {
                // 1:1 to AcrRequest via unique AcrRequestId
                e.HasOne(x => x.AcrRequest)
                 .WithMany()
                 .HasForeignKey(x => x.AcrRequestId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => x.AcrRequestId).IsUnique();

                // Each day → OvertimeTypes (optional; Restrict deletions)
                e.HasOne(x => x.MondayType)
                 .WithMany()
                 .HasForeignKey(x => x.MondayTypeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_AcrOvertimeSchedules_MondayType_OvertimeTypes");

                e.HasOne(x => x.TuesdayType)
                 .WithMany()
                 .HasForeignKey(x => x.TuesdayTypeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_AcrOvertimeSchedules_TuesdayType_OvertimeTypes");

                e.HasOne(x => x.WednesdayType)
                 .WithMany()
                 .HasForeignKey(x => x.WednesdayTypeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_AcrOvertimeSchedules_WednesdayType_OvertimeTypes");

                e.HasOne(x => x.ThursdayType)
                 .WithMany()
                 .HasForeignKey(x => x.ThursdayTypeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_AcrOvertimeSchedules_ThursdayType_OvertimeTypes");

                e.HasOne(x => x.FridayType)
                 .WithMany()
                 .HasForeignKey(x => x.FridayTypeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_AcrOvertimeSchedules_FridayType_OvertimeTypes");

                e.HasOne(x => x.SaturdayType)
                 .WithMany()
                 .HasForeignKey(x => x.SaturdayTypeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_AcrOvertimeSchedules_SaturdayType_OvertimeTypes");

                e.HasOne(x => x.SundayType)
                 .WithMany()
                 .HasForeignKey(x => x.SundayTypeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_AcrOvertimeSchedules_SundayType_OvertimeTypes");
            });
        }
    }
}
