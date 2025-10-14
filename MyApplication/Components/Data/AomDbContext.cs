using Microsoft.EntityFrameworkCore;

// Employee / ACR / Opera models
using MyApplication.Components.Model.AOM.Employee;
// Tools models
using MyApplication.Components.Model.AOM.Tools;

namespace MyApplication.Components.Data
{
    public class AomDbContext : DbContext
    {
        public AomDbContext(DbContextOptions<AomDbContext> options) : base(options) { }

        // ---------- Employee schema ----------
        public DbSet<Employees>        Employees          { get; set; } = default!;
        public DbSet<Employer>         Employers          { get; set; } = default!;
        public DbSet<Manager>          Managers           { get; set; } = default!;
        public DbSet<Supervisor>       Supervisors        { get; set; } = default!;
        public DbSet<Site>             Sites              { get; set; } = default!;
        public DbSet<Organization>     Organizations      { get; set; } = default!;
        public DbSet<SubOrganization>  SubOrganizations   { get; set; } = default!;
        public DbSet<EmployeeHistory>  EmployeeHistories  { get; set; } = default!;
        public DbSet<Schedule>         Schedules          { get; set; } = default!; // assumes Schedule has Id

        // ---------- Opera (Employee schema) ----------
        public DbSet<OperaType>        OperaType          { get; set; } = default!;
        public DbSet<OperaSubType>     OperaSubType       { get; set; } = default!;
        public DbSet<OperaSubClass>    OperaSubClass      { get; set; } = default!;
        public DbSet<OperaStatus>      OperaStatus        { get; set; } = default!;
        public DbSet<OperaRequest>     OperaRequest       { get; set; } = default!;

        // ---------- ACR (Employee schema) ----------
        public DbSet<AcrType>          AcrTypes           { get; set; } = default!;
        public DbSet<AcrStatus>        AcrStatus          { get; set; } = default!;
        public DbSet<AcrRequest>       AcrRequests        { get; set; } = default!;
        public DbSet<AcrSchedule>      AcrSchedule        { get; set; } = default!;
        public DbSet<AcrOrganization>  AcrOrganization    { get; set; } = default!;

        // ---------- Tools schema ----------
        public DbSet<EmailTemplates>         EmailTemplates         { get; set; } = default!;
        public DbSet<IntervalSummary>        IntervalSummaries      { get; set; } = default!;
        public DbSet<OiCategory>             OiCategories           { get; set; } = default!;
        public DbSet<OiSeverity>             OiSeverities           { get; set; } = default!;
        public DbSet<OiEvent>                OiEvents               { get; set; } = default!;
        public DbSet<OiEventUpdate>          OiEventUpdates         { get; set; } = default!;
        public DbSet<OiStatus>               OiStatuses             { get; set; } = default!;
        public DbSet<ProactiveAnnouncement>  ProactiveAnnouncements { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // =========================
            // Tables + Primary Keys
            // =========================
            b.Entity<Employees>(e        => { e.ToTable(nameof(Employees), "Employee");       e.HasKey(x => x.Id); });
            b.Entity<Employer>(e         => { e.ToTable(nameof(Employer), "Employee");        e.HasKey(x => x.Id); });
            b.Entity<Manager>(e          => { e.ToTable(nameof(Manager), "Employee");         e.HasKey(x => x.Id); });
            b.Entity<Supervisor>(e       => { e.ToTable(nameof(Supervisor), "Employee");      e.HasKey(x => x.Id); });
            b.Entity<Site>(e             => { e.ToTable(nameof(Site), "Employee");            e.HasKey(x => x.Id); });
            b.Entity<Organization>(e     => { e.ToTable(nameof(Organization), "Employee");    e.HasKey(x => x.Id); });
            b.Entity<SubOrganization>(e  => { e.ToTable(nameof(SubOrganization), "Employee"); e.HasKey(x => x.Id); });
            b.Entity<EmployeeHistory>(e  => { e.ToTable(nameof(EmployeeHistory), "Employee"); e.HasKey(x => x.Id); });
            b.Entity<Schedule>(e         => { e.ToTable(nameof(Schedule), "Employee");        e.HasKey(x => x.Id); });

            b.Entity<OperaType>(e        => { e.ToTable(nameof(OperaType), "Employee");       e.HasKey(x => x.Id); });
            b.Entity<OperaSubType>(e     => { e.ToTable(nameof(OperaSubType), "Employee");    e.HasKey(x => x.Id); });
            b.Entity<OperaSubClass>(e    => { e.ToTable(nameof(OperaSubClass), "Employee");   e.HasKey(x => x.Id); });
            b.Entity<OperaStatus>(e      => { e.ToTable(nameof(OperaStatus), "Employee");     e.HasKey(x => x.Id); });
            b.Entity<OperaRequest>(e     => { e.ToTable(nameof(OperaRequest), "Employee");    e.HasKey(x => x.RequestId); });

            b.Entity<AcrType>(e          => { e.ToTable(nameof(AcrType), "Employee");         e.HasKey(x => x.Id); });
            b.Entity<AcrStatus>(e        => { e.ToTable(nameof(AcrStatus), "Employee");       e.HasKey(x => x.Id); });
            b.Entity<AcrRequest>(e       => { e.ToTable(nameof(AcrRequest), "Employee");      e.HasKey(x => x.Id); });
            b.Entity<AcrSchedule>(e      => { e.ToTable(nameof(AcrSchedule), "Employee");     e.HasKey(x => x.Id); });
            b.Entity<AcrOrganization>(e  => { e.ToTable(nameof(AcrOrganization), "Employee"); e.HasKey(x => x.Id); });

            b.Entity<EmailTemplates>(e         => { e.ToTable(nameof(EmailTemplates), "Tools");         e.HasKey(x => x.Id); });
            b.Entity<IntervalSummary>(e        => { e.ToTable(nameof(IntervalSummary), "Tools");        e.HasKey(x => x.Id); });
            b.Entity<OiCategory>(e             => { e.ToTable(nameof(OiCategory), "Tools");             e.HasKey(x => x.Id); });
            b.Entity<OiSeverity>(e             => { e.ToTable(nameof(OiSeverity), "Tools");             e.HasKey(x => x.Id); });
            b.Entity<OiEvent>(e                => { e.ToTable(nameof(OiEvent), "Tools");                e.HasKey(x => x.Id); });
            b.Entity<OiEventUpdate>(e          => { e.ToTable(nameof(OiEventUpdate), "Tools");          e.HasKey(x => x.Id); });
            b.Entity<OiStatus>(e               => { e.ToTable(nameof(OiStatus), "Tools");               e.HasKey(x => x.Id); });
            b.Entity<ProactiveAnnouncement>(e  => { e.ToTable(nameof(ProactiveAnnouncement), "Tools");  e.HasKey(x => x.Id); });

            // =========================
            // Relationships: Employee / HR
            // =========================
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

                e.HasOne(x => x.Schedule)
                 .WithMany()
                 .HasForeignKey(x => x.ScheduleId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.EmployeeId, x.EffectiveDate });
            });

            // =========================
            // Relationships: Opera
            // =========================
            b.Entity<OperaSubType>()
             .HasOne(st => st.OperaType)
             .WithMany(t => t.SubTypes)                  // bind to real collection
             .HasForeignKey(st => st.OperaTypeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<OperaSubClass>()
             .HasOne(sc => sc.OperaSubType)
             .WithMany(st => st.SubClasses)              // bind to real collection
             .HasForeignKey(sc => sc.OperaSubTypeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<OperaRequest>()
             .HasOne(r => r.OperaType)
             .WithMany()
             .HasForeignKey(r => r.OperaTypeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<OperaRequest>()
             .HasOne(r => r.OperaSubType)
             .WithMany()
             .HasForeignKey(r => r.OperaSubTypeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<OperaRequest>()
             .HasOne(r => r.OperaSubClass)
             .WithMany()
             .HasForeignKey(r => r.OperaSubClassId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<OperaRequest>()
             .HasOne(r => r.Employees)                   // nav is named Employees (single)
             .WithMany()
             .HasForeignKey(r => r.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Entity<OperaRequest>().HasIndex(r => r.StartTime);
            b.Entity<OperaRequest>().HasIndex(r => new { r.EmployeeId, r.StartTime });

            // =========================
            // Relationships: ACR
            // =========================
            b.Entity<AcrRequest>(e =>
            {
                e.ToTable(nameof(AcrRequest), "Employee");
                e.HasKey(x => x.Id);

                e.HasOne(r => r.Employee)     // ✅ bind to NAV
                 .WithMany()
                 .HasForeignKey(r => r.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.AcrType)      // ✅ bind to NAV
                 .WithMany()
                 .HasForeignKey(r => r.AcrTypeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.AcrStatus)    // ✅ bind to NAV
                 .WithMany()
                 .HasForeignKey(r => r.AcrStatusId)
                 .OnDelete(DeleteBehavior.Restrict);

                // helpful indexes for your filters
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

                // Optional lookup FKs (uncomment if those entities exist and you want FKs enforced)
                e.HasOne(o => o.Manager).WithMany().HasForeignKey(o => o.ManagerId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(o => o.Supervisor).WithMany().HasForeignKey(o => o.SupervisorId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(o => o.Organization).WithMany().HasForeignKey(o => o.OrganizationId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(o => o.SubOrganization).WithMany().HasForeignKey(o => o.SubOrganizationId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(o => o.Employer).WithMany().HasForeignKey(o => o.EmployerId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(o => o.Site).WithMany().HasForeignKey(o => o.SiteId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            });

            b.Entity<AcrSchedule>(e =>
            {
                e.Property(x => x.ShiftNumber).IsRequired(); // 1 or 2

                e.HasOne(s => s.AcrRequest)
                 .WithMany()                                  // allows two rows per request (split)
                 .HasForeignKey(s => s.AcrRequestId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Optional uniqueness guard (would require a migration):
                // e.HasIndex(s => new { s.AcrRequestId, s.ShiftNumber }).IsUnique();
            });

            // =========================
            // Tools schema tables only (no relationships defined here)
            // =========================
            // Already configured above with table + key
        }
    }
}
