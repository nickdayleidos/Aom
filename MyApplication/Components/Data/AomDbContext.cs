using Microsoft.EntityFrameworkCore;

// Employee & Opera domain (your AOM.Employee namespace)
using MyApplication.Components.Model.AOM.Employee;

// Tools domain
using MyApplication.Components.Model.AOM.Tools;


namespace MyApplication.Components.Data
{
    public class AomDbContext : DbContext
    {
        public AomDbContext(DbContextOptions<AomDbContext> options) : base(options) { }

        // -------- Employee schema entities --------
        public DbSet<Employees> Employees { get; set; } = default!;
        public DbSet<Employer> Employers { get; set; } = default!;
        public DbSet<Manager> Managers { get; set; } = default!;
        public DbSet<Supervisor> Supervisors { get; set; } = default!;
        public DbSet<Site> Sites { get; set; } = default!;
        public DbSet<Organization> Organizations { get; set; } = default!;
        public DbSet<SubOrganization> SubOrganizations { get; set; } = default!;
        public DbSet<EmployeeHistory> EmployeeHistories { get; set; } = default!;
        public DbSet<Schedule> Schedules { get; set; } = default!;

        // Opera
        public DbSet<OperaRequest> OperaRequest { get; set; } = default!;
        public DbSet<OperaType> OperaType { get; set; } = default!;
        public DbSet<OperaSubType> OperaSubType { get; set; } = default!;
        public DbSet<OperaSubClass> OperaSubClass { get; set; } = default!;
        public DbSet<OperaStatus> OperaStatus { get; set; } = default!;

        // Acr

        public DbSet<AcrType> AcrTypes { get; set; } = default!;
        public DbSet<AcrRequest> AcrRequests { get; set; } = default!;
        public DbSet<AcrStatus> AcrStatus { get; set; } = default!;
        public DbSet<AcrSchedule> AcrSchedule { get; set; } = default!;
        public DbSet<AcrOrganization> AcrOrganization { get; set; } = default!;


        // -------- Tools schema entities --------
        public DbSet<EmailTemplates> EmailTemplates { get; set; } = default!;
        public DbSet<IntervalSummary> IntervalSummaries { get; set; } = default!;
        public DbSet<OiCategory> OiCategories { get; set; } = default!;
        public DbSet<OiSeverity> OiSeverities { get; set; } = default!;
        public DbSet<OiEvent> OiEvents { get; set; } = default!;
        public DbSet<OiEventUpdate> OiEventUpdates { get; set; } = default!;
        public DbSet<OiStatus> OiStatuses { get; set; } = default!;
        public DbSet<ProactiveAnnouncement> ProactiveAnnouncements { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ----- Tables -----
            b.Entity<Employees>(e => e.ToTable(nameof(Employees), "Employee"));
            b.Entity<Employer>(e => e.ToTable(nameof(Employer), "Employee"));
            b.Entity<Manager>(e => e.ToTable(nameof(Manager), "Employee"));
            b.Entity<Supervisor>(e => e.ToTable(nameof(Supervisor), "Employee"));
            b.Entity<Site>(e => e.ToTable(nameof(Site), "Employee"));
            b.Entity<Organization>(e => e.ToTable(nameof(Organization), "Employee"));
            b.Entity<SubOrganization>(e => e.ToTable(nameof(SubOrganization), "Employee"));
            b.Entity<EmployeeHistory>(e => e.ToTable(nameof(EmployeeHistory), "Employee"));
            b.Entity<Schedule>(e => e.ToTable(nameof(Schedule), "Employee"));

            b.Entity<OperaType>(e => e.ToTable(nameof(OperaType), "Employee"));
            b.Entity<OperaSubType>(e => e.ToTable(nameof(OperaSubType), "Employee"));
            b.Entity<OperaSubClass>(e => e.ToTable(nameof(OperaSubClass), "Employee"));
            b.Entity<OperaStatus>(e => e.ToTable(nameof(OperaStatus), "Employee"));
            b.Entity<OperaRequest>(e => e.ToTable(nameof(OperaRequest), "Employee"));

            b.Entity<AcrType>(e => e.ToTable(nameof(AcrType), "Employee"));
            b.Entity<AcrStatus>(e => e.ToTable(nameof(AcrStatus), "Employee"));
            b.Entity<AcrRequest>(e => e.ToTable(nameof(AcrRequest), "Employee"));
            b.Entity<AcrSchedule>(e => e.ToTable(nameof(AcrSchedule), "Employee"));
            b.Entity<AcrOrganization>(e => e.ToTable(nameof(AcrOrganization), "Employee"));

            // ----- Relationships (NO CASCADE) -----

            // OperaSubType -> OperaType
            // If your OperaType has: public ICollection<OperaSubType> SubTypes { get; set; }
            b.Entity<OperaSubType>()
                .HasOne(st => st.OperaType)
                .WithMany(t => t.SubTypes)          // <— tie to the collection nav (prevents a 2nd, shadow relationship)
                .HasForeignKey(st => st.OperaTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // OperaSubClass -> OperaSubType
            // If your OperaSubType has: public ICollection<OperaSubClass> SubClasses { get; set; }
            b.Entity<OperaSubClass>()
                .HasOne(sc => sc.OperaSubType)
                .WithMany(st => st.SubClasses)      // <— tie to the collection nav
                .HasForeignKey(sc => sc.OperaSubTypeId)
                .OnDelete(DeleteBehavior.Restrict);



            // OperaRequest -> OperaType
            b.Entity<OperaRequest>()
                .HasOne(r => r.OperaType)
                .WithMany()
                .HasForeignKey(r => r.OperaTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // OperaRequest -> OperaSubType
            b.Entity<OperaRequest>()
                .HasOne(r => r.OperaSubType)
                .WithMany()
                .HasForeignKey(r => r.OperaSubTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // OperaRequest -> OperaSubClass (nullable)
            b.Entity<OperaRequest>()
                .HasOne(r => r.OperaSubClass)
                .WithMany()
                .HasForeignKey(r => r.OperaSubClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for list/search
            b.Entity<OperaRequest>().HasIndex(r => r.StartTime);
            b.Entity<OperaRequest>().HasIndex(r => new { r.EmployeeId, r.StartTime });

            // ------- Tools schema (tables) -------
            b.Entity<EmailTemplates>(e => e.ToTable(nameof(EmailTemplates), "Tools"));
            b.Entity<IntervalSummary>(e => e.ToTable(nameof(IntervalSummary), "Tools"));
            b.Entity<OiCategory>(e => e.ToTable(nameof(OiCategory), "Tools"));
            b.Entity<OiSeverity>(e => e.ToTable(nameof(OiSeverity), "Tools"));
            b.Entity<OiEvent>(e => e.ToTable(nameof(OiEvent), "Tools"));
            b.Entity<OiEventUpdate>(e => e.ToTable(nameof(OiEventUpdate), "Tools"));
            b.Entity<OiStatus>(e => e.ToTable(nameof(OiStatus), "Tools"));
            b.Entity<ProactiveAnnouncement>(e => e.ToTable(nameof(ProactiveAnnouncement), "Tools"));
        }
    }
    }
