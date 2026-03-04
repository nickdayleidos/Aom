using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AOM.Aws;
using MyApplication.Components.Model.AOM.Employee;
using MyApplication.Components.Model.AOM.Security;
using MyApplication.Components.Model.AOM.Tools;
using MyApplication.Components.Service.Employee;

namespace MyApplication.Components.Data
{
    // Entity configs split into partial files:
    //   AomDbContext.Employee.cs  – Employee schema
    //   AomDbContext.Tools.cs     – Tools schema
    //   AomDbContext.Aws.cs       – AWS schema
    //   AomDbContext.Security.cs  – Security schema
    public partial class AomDbContext : DbContext
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

        // Certifications
        public DbSet<CertificationType> CertificationTypes { get; set; } = default!;
        public DbSet<CertificationVendor> CertificationVendors { get; set; } = default!;
        public DbSet<Certification> Certifications { get; set; } = default!;

        // NEW: View
        public DbSet<EmployeeCurrentDetails> EmployeeCurrentDetails { get; set; } = default!;

        // WFM (Employee schema)
        public DbSet<BreakTemplates> BreakTemplates { get; set; } = default!;
        public DbSet<AcrOvertimeSchedules> AcrOvertimeSchedules { get; set; } = default!;
        public DbSet<AcrOvertimeTypes> AcrOvertimeTypes { get; set; } = default!;
        public DbSet<BreakSchedules> BreakSchedules { get; set; } = default!;
        public DbSet<DetailedSchedule> DetailedSchedule { get; set; } = default!;
        public DbSet<ScheduleType> ScheduleType { get; set; } = default!;

        // Opera (Employee schema)
        public DbSet<ActivityType> ActivityTypes { get; set; } = default!;
        public DbSet<ActivitySubType> ActivitySubTypes { get; set; } = default!;
        public DbSet<OperaStatus> OperaStatuses { get; set; } = default!;
        public DbSet<OperaRequest> OperaRequests { get; set; } = default!;
        public DbSet<OperaTimeframe> OperaTimeframe { get; set; } = default!;

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
        public DbSet<OstPassdown> OstPassdown { get; set; }

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

            ConfigureEmployeeSchema(b);
            ConfigureToolsSchema(b);
            ConfigureAwsSchema(b);
            ConfigureSecuritySchema(b);
        }

        partial void ConfigureEmployeeSchema(ModelBuilder b);
        partial void ConfigureToolsSchema(ModelBuilder b);
        partial void ConfigureAwsSchema(ModelBuilder b);
        partial void ConfigureSecuritySchema(ModelBuilder b);
    }
}